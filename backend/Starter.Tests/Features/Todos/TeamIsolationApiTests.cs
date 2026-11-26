using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>
/// MB-1: en säljare försöker läsa och ändra ekonomis uppgifter genom att byta id i adressen (IDOR).
/// Varje försök ska ge 404, så att det inte ens går att se att uppgiften finns, och ekonomis uppgift ska vara orörd.
/// </summary>
public sealed class TeamIsolationApiTests : IClassFixture<TodosApiTests.ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    private readonly HttpClient _ekonomi;
    private readonly HttpClient _salj;

    public TeamIsolationApiTests(TodosApiTests.ApiFactory factory)
    {
        _ekonomi = factory.CreateClient();
        _ekonomi.DefaultRequestHeaders.Add("X-Team", "ekonomi");
        _salj = factory.CreateClient();
        _salj.DefaultRequestHeaders.Add("X-Team", "salj");
    }

    [Fact]
    [Trait("Abuse", "MB-1.1")]
    public async Task Another_team_cannot_read_a_todo_by_id()
    {
        var secret = await CreateEkonomiTodoAsync("Lönekörning (konfidentiellt)");

        var response = await _salj.GetAsync($"/api/todos/{secret.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.DoesNotContain("Lönekörning", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    [Trait("Abuse", "MB-1.2")]
    public async Task Another_team_cannot_update_a_todo_by_id()
    {
        var secret = await CreateEkonomiTodoAsync("Bonusunderlag");

        var response = await _salj.PutAsJsonAsync($"/api/todos/{secret.Id}", new { title = "Hackad av sälj" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("Bonusunderlag", (await GetAsEkonomiAsync(secret.Id)).Title);
    }

    [Fact]
    [Trait("Abuse", "MB-1.3")]
    public async Task Another_team_cannot_delete_a_todo_by_id()
    {
        var secret = await CreateEkonomiTodoAsync("Momsdeklaration");

        var response = await _salj.DeleteAsync($"/api/todos/{secret.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _ekonomi.GetAsync($"/api/todos/{secret.Id}")).StatusCode);
    }

    [Fact]
    [Trait("Abuse", "MB-1.4")]
    public async Task Another_team_cannot_mark_a_todo_as_done()
    {
        var secret = await CreateEkonomiTodoAsync("Bokslut");

        var response = await _salj.PutAsync($"/api/todos/{secret.Id}/done", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.False((await GetAsEkonomiAsync(secret.Id)).IsDone);
    }

    private async Task<TodoResponse> CreateEkonomiTodoAsync(string title)
    {
        var created = await _ekonomi.PostAsJsonAsync("/api/todos", new { title });
        created.EnsureSuccessStatusCode();
        return (await created.Content.ReadFromJsonAsync<TodoResponse>(Json))!;
    }

    private async Task<TodoResponse> GetAsEkonomiAsync(int id) =>
        (await _ekonomi.GetFromJsonAsync<TodoResponse>($"/api/todos/{id}", Json))!;
}
