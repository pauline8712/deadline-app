using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>US-2: se vilka uppgifter som är försenade. Uppgifterna skapas söndag 2026-11-15 och klockan spolas sedan fram, vintertid (UTC+1).</summary>
public sealed class OverdueTests
{
    private static readonly DateOnly Monday = new(2026, 11, 16);

    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 11, 15, 8, 0, 0, TimeSpan.Zero));
    private readonly FakeTodoRepository _repository = new();
    private readonly TodoService _service;

    public OverdueTests()
    {
        _service = new TodoService(_repository, _clock);
    }

    [Fact]
    [Trait("Krav", "AK-2.2")]
    public async Task A_todo_is_one_day_late_at_midnight_after_its_due_date()
    {
        var todo = await CreateAsync("Skicka offert", Monday);

        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 23, 0, 0, TimeSpan.Zero)); // 00:00 den 17:e i Sverige
        var overdue = await ListOverdueAsync();

        var late = Assert.Single(overdue, t => t.Id == todo.Id);
        Assert.Equal(1, late.DaysLate);
    }

    [Fact]
    [Trait("Krav", "AK-2.3")]
    public async Task A_done_todo_is_never_overdue()
    {
        var todo = await CreateAsync("Skicka offert", new DateOnly(2026, 11, 15));
        await _service.MarkDoneAsync(todo.Id, CancellationToken.None);

        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 8, 0, 0, TimeSpan.Zero));
        var overdue = await ListOverdueAsync();

        Assert.DoesNotContain(overdue, t => t.Id == todo.Id);
    }

    [Fact]
    [Trait("Krav", "IK-3")]
    public async Task Swedish_time_decides_the_day_not_the_server_time()
    {
        var todo = await CreateAsync("Skicka offert", Monday);

        _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 23, 30, 0, TimeSpan.Zero)); // fortfarande den 16:e i UTC, men 00:30 den 17:e i Sverige
        var overdue = await ListOverdueAsync();

        Assert.Contains(overdue, t => t.Id == todo.Id);
    }

    private async Task<TodoResponse> CreateAsync(string title, DateOnly dueDate)
    {
        var result = await _service.CreateAsync(new CreateTodoRequest(title, dueDate), CancellationToken.None);
        return result.Data!;
    }

    private async Task<IReadOnlyList<OverdueTodoResponse>> ListOverdueAsync() =>
        (await _service.ListOverdueAsync(CancellationToken.None)).Data!;
}
