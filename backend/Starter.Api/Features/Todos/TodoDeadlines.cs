namespace Starter.Api.Features.Todos;

/// <summary>Regeln för när en uppgift är försenad. Ren: den får tiden som parameter och läser aldrig klockan själv.</summary>
public static class TodoDeadlines
{
    public static bool IsOverdue(DateTime? dueDate, bool isDone, DateTime now) =>
        dueDate != null && dueDate < now && !isDone;

    public static int DaysLate(DateTime dueDate, DateTime now) => (now - dueDate).Days;
}
