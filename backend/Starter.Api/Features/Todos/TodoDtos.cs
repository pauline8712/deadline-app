namespace Starter.Api.Features.Todos;

/// <summary>What a client sends to create a todo.</summary>
public sealed record CreateTodoRequest(string Title, DateOnly? DueDate = null);

/// <summary>What a client gets back. This is the shape the frontend depends on.</summary>
public sealed record TodoResponse(int Id, string Title, bool IsDone, DateTimeOffset CreatedAt, DateOnly? DueDate);

/// <summary>Det som behövs för att avgöra om en klar uppgift blev klar i tid. Hämtas som en smal projektion.</summary>
public sealed record CompletedDeadline(DateOnly DueDate, DateTimeOffset CompletedAt);

/// <summary>Saras mätvärde (US-4). <c>OnTimeRate</c> är null när det inte finns någon data än, eftersom det inte är samma sak som 0 %.</summary>
public sealed record TodoStatsResponse(int DoneOnTime, int DoneLate, int OverdueNow, double? OnTimeRate);

/// <summary>En försenad uppgift, och hur många dagar sen den är.</summary>
public sealed record OverdueTodoResponse(int Id, string Title, DateOnly DueDate, int DaysLate);
