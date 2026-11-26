namespace Starter.Api.Features.Todos;

/// <summary>Regeln för när en uppgift är försenad. Ren: den får dagens datum som parameter och läser aldrig klockan själv.</summary>
public static class TodoDeadlines
{
    public static bool IsOverdue(DateOnly? dueDate, bool isDone, DateOnly today) =>
        dueDate != null && dueDate <= today && !isDone;

    public static int DaysLate(DateOnly dueDate, DateOnly today) => today.DayNumber - dueDate.DayNumber;
}
