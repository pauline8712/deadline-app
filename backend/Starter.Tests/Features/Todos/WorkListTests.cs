using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>US-3: arbetslistan (GET /api/todos). Uppgifterna skapas torsdag 2026-11-12, och klockan spolas sedan fram till måndag 2026-11-16.</summary>
public sealed class WorkListTests
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 11, 12, 8, 0, 0, TimeSpan.Zero));
    private readonly FakeTodoRepository _repository = new();
    private readonly TodoService _service;

    public WorkListTests()
    {
        _service = new TodoService(_repository, _clock);
    }

    [Fact]
    [Trait("Krav", "AK-3.1")]
    public async Task Overdue_comes_first_then_today_then_later()
    {
        await CreateAsync("I morgon", new DateOnly(2026, 11, 17));
        await CreateAsync("Försenad", new DateOnly(2026, 11, 13));
        await CreateAsync("Idag", new DateOnly(2026, 11, 16));
        GoToMonday();

        var titles = await ListTitlesAsync();

        Assert.Equal(["Försenad", "Idag", "I morgon"], titles);
    }

    [Fact]
    [Trait("Krav", "AK-3.2")]
    public async Task Todos_without_a_due_date_come_after_todos_with_one()
    {
        await CreateAsync("Utan datum", null);
        await CreateAsync("Fredag", new DateOnly(2026, 11, 20));
        GoToMonday();

        var titles = await ListTitlesAsync();

        Assert.Equal(["Fredag", "Utan datum"], titles);
    }

    private async Task<TodoResponse> CreateAsync(string title, DateOnly? dueDate) =>
        (await _service.CreateAsync(new CreateTodoRequest(title, dueDate), CancellationToken.None)).Data!;

    private void GoToMonday() => _clock.SetUtcNow(new DateTimeOffset(2026, 11, 16, 8, 0, 0, TimeSpan.Zero));

    private async Task<List<string>> ListTitlesAsync() =>
        (await _service.ListAsync(CancellationToken.None)).Data!.Select(t => t.Title).ToList();
}
