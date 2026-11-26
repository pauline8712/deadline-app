using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>
/// MB-2: en säljare skriver SQL i sökfältet för att se alla teams uppgifter.
/// Söktexten ska alltid behandlas som text, aldrig som en del av SQL-satsen.
/// </summary>
public sealed class SearchInjectionApiTests : IClassFixture<TodosApiTests.ApiFactory>
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    private readonly HttpClient _ekonomi;
    private readonly HttpClient _salj;

    public SearchInjectionApiTests(TodosApiTests.ApiFactory factory)
    {
        _ekonomi = factory.CreateClient();
        _ekonomi.DefaultRequestHeaders.Add("X-Team", "ekonomi");
        _salj = factory.CreateClient();
        _salj.DefaultRequestHeaders.Add("X-Team", "salj");
    }

    [Fact]
    [Trait("Abuse", "MB-2.1")]
    public async Task Sql_in_the_search_box_is_treated_as_text()
    {
        var secret = await CreateAsync(_ekonomi, "Lönekörning (konfidentiellt)");

        var response = await _salj.GetAsync("/api/todos/search?q=" + Uri.EscapeDataString("' OR 1=1 --"));
        var hits = await response.Content.ReadFromJsonAsync<List<TodoResponse>>(Json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain(hits!, t => t.Id == secret.Id);
    }

    [Fact]
    [Trait("Abuse", "MB-2.2")]
    public async Task A_normal_search_only_finds_the_callers_own_team()
    {
        var own = await CreateAsync(_salj, "Skicka offert till kund");
        var other = await CreateAsync(_ekonomi, "Godkänn offert för revision");

        var hits = await _salj.GetFromJsonAsync<List<TodoResponse>>("/api/todos/search?q=offert", Json);

        Assert.Contains(hits!, t => t.Id == own.Id);
        Assert.DoesNotContain(hits!, t => t.Id == other.Id);
    }

    private static async Task<TodoResponse> CreateAsync(HttpClient client, string title)
    {
        var created = await client.PostAsJsonAsync("/api/todos", new { title });
        created.EnsureSuccessStatusCode();
        return (await created.Content.ReadFromJsonAsync<TodoResponse>(Json))!;
    }
}
