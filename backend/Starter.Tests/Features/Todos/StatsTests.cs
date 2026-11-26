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

    [Fact]
    [Trait("Krav", "AK-4.1")]
    public async Task A_todo_done_late_on_its_due_date_is_done_on_time()
    {
        var todo = await CreateAsync("Skicka offert", Monday);

        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 22, 0, 0, TimeSpan.Zero)); // 23:00 den 16:e i Sverige
        await _service.MarkDoneAsync(todo.Id, CancellationToken.None);
        var stats = await GetStatsAsync();

        Assert.Equal(_clock.GetUtcNow(), _repository.Saved.Single().CompletedAt);
        Assert.Equal(1, stats.DoneOnTime);
        Assert.Equal(0, stats.DoneLate);
    }

    private async Task<TodoResponse> CreateAsync(string title, DateOnly? dueDate) =>
        (await _service.CreateAsync(new CreateTodoRequest(title, dueDate), CancellationToken.None)).Data!;

    private async Task<TodoStatsResponse> GetStatsAsync() =>
        (await _service.GetStatsAsync(CancellationToken.None)).Data!;
}
