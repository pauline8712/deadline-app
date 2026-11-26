using System.Net.Http.Json;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>US-4 genom hela API:t, med en egen tom databas så att inga andra tester påverkar siffrorna.</summary>
public sealed class StatsApiTests(TodosApiTests.ApiFactory factory) : IClassFixture<TodosApiTests.ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    [Trait("Krav", "AK-4.4")]
    [Trait("Krav", "AK-4.1")]
    public async Task Stats_say_no_data_until_something_is_done_then_count_it()
    {
        var empty = await _client.GetStringAsync("/api/todos/stats");
        Assert.Contains("\"onTimeRate\":null", empty);

        factory.Clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 8, 0, 0, TimeSpan.Zero));
        var created = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Skicka offert", new DateOnly(2026, 11, 16)));
        var todo = await created.Content.ReadFromJsonAsync<TodoResponse>();
        factory.Clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 22, 0, 0, TimeSpan.Zero)); // 23:00 i Sverige
        await _client.PutAsync($"/api/todos/{todo!.Id}/done", null);

        var stats = await _client.GetFromJsonAsync<TodoStatsResponse>("/api/todos/stats");

        Assert.Equal(new TodoStatsResponse(DoneOnTime: 1, DoneLate: 0, OverdueNow: 0, OnTimeRate: 1.0), stats);
    }
}
