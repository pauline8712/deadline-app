using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>Ordningen i arbetslistan (US-3). Ren, så att den går att testa och mäta med BenchmarkDotNet (IK-2).</summary>
public static class TodoOrdering
{
    /// <summary>
    /// Ej klara först, sorterade på förfallodatum så att försenade kommer före dagens och dagens före senare
    /// (AK-3.1). Uppgifter utan förfallodatum kommer efter dem med datum (AK-3.2), och klara uppgifter sist
    /// (AK-3.3).
    /// </summary>
    public static IEnumerable<Todo> ForWorkList(IEnumerable<Todo> todos) =>
        todos
            .OrderBy(t => t.IsDone)
            .ThenBy(t => t.DueDate == null)
            .ThenBy(t => t.DueDate);
}
