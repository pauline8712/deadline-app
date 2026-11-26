using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

public sealed class TodoDeadlinesTests
{
    private static readonly DateOnly Today = new(2026, 11, 16);

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public void A_todo_due_today_is_not_overdue()
    {
        var todo = new Todo { Title = "Skicka offert", DueDate = Today };

        Assert.False(TodoDeadlines.IsOverdue(todo, Today));
    }
}
