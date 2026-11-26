using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>US-4: följa upp målet. Uppgifterna skapas torsdag 2026-11-12 och klockan spolas sedan fram. Vintertid (UTC+1).</summary>
public sealed class StatsTests
{
    private static readonly DateOnly Monday = new(2026, 11, 16);

    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 11, 12, 8, 0, 0, TimeSpan.Zero));
    private readonly FakeTodoRepository _repository = new();
    private readonly TodoService _service;

    public StatsTests()
    {
        _service = new TodoService(_repository, _clock);
    }

    [Fact]
    [Trait("Krav", "AK-4.4")]
    public async Task No_data_yet_is_not_shown_as_zero_percent()
    {
        await CreateAsync("Inte klar än", Monday);

        var stats = await GetStatsAsync();

        Assert.Null(stats.OnTimeRate);
    }

    private async Task<TodoResponse> CreateAsync(string title, DateOnly? dueDate) =>
        (await _service.CreateAsync(new CreateTodoRequest(title, dueDate), CancellationToken.None)).Data!;

    private async Task<TodoStatsResponse> GetStatsAsync() =>
        (await _service.GetStatsAsync(CancellationToken.None)).Data!;
}
