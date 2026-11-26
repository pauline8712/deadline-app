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

    [Fact]
    [Trait("Krav", "AK-4.2")]
    public async Task A_todo_done_the_day_after_its_due_date_is_done_late()
    {
        var todo = await CreateAsync("Skicka offert", Monday);

        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 17, 7, 0, 0, TimeSpan.Zero)); // 08:00 den 17:e i Sverige
        await _service.MarkDoneAsync(todo.Id, CancellationToken.None);
        var stats = await GetStatsAsync();

        Assert.Equal(0, stats.DoneOnTime);
        Assert.Equal(1, stats.DoneLate);
    }

    [Fact]
    [Trait("Krav", "AK-4.3")]
    public async Task Stats_count_done_on_time_done_late_overdue_now_and_the_rate()
    {
        var onTime = new[] { await CreateAsync("I tid 1", Monday), await CreateAsync("I tid 2", Monday), await CreateAsync("I tid 3", Monday) };
        var late = await CreateAsync("Sen", new DateOnly(2026, 11, 13));
        await CreateAsync("Försenad 1", new DateOnly(2026, 11, 13));
        await CreateAsync("Försenad 2", new DateOnly(2026, 11, 13));
        foreach (var todo in onTime)
        {
            await _service.MarkDoneAsync(todo.Id, CancellationToken.None);
        }

        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 14, 8, 0, 0, TimeSpan.Zero));
        await _service.MarkDoneAsync(late.Id, CancellationToken.None);
        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 8, 0, 0, TimeSpan.Zero));
        var stats = await GetStatsAsync();

        Assert.Equal(new TodoStatsResponse(DoneOnTime: 3, DoneLate: 1, OverdueNow: 2, OnTimeRate: 0.75), stats);
    }

    private async Task<TodoResponse> CreateAsync(string title, DateOnly? dueDate) =>
        (await _service.CreateAsync(new CreateTodoRequest(title, dueDate), CancellationToken.None)).Data!;

    private async Task<TodoStatsResponse> GetStatsAsync() =>
        (await _service.GetStatsAsync(CancellationToken.None)).Data!;
}
