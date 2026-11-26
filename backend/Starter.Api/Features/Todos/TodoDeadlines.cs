using System.Linq.Expressions;
using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>Regeln för när en uppgift är försenad. Ren: den får dagens datum som parameter och läser aldrig klockan själv.</summary>
public static class TodoDeadlines
{
    /// <summary>Regeln som ett uttryck, så att EF Core kan översätta den till SQL och filtrera i databasen (IK-4).</summary>
    public static Expression<Func<Todo, bool>> IsOverdueOn(DateOnly today) =>
        todo => todo.DueDate != null && todo.DueDate < today && !todo.IsDone;

    /// <summary>Samma regel för en enskild uppgift i minnet.</summary>
    public static bool IsOverdue(Todo todo, DateOnly today) => IsOverdueOn(today).Compile()(todo);

    public static int DaysLate(DateOnly dueDate, DateOnly today) => today.DayNumber - dueDate.DayNumber;
}
