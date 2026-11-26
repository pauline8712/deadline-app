using BenchmarkDotNet.Attributes;
using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Benchmarks;

/// <summary>IK-2: sorteringen av arbetslistan ska ta under 1 ms för 1 000 uppgifter.</summary>
[MemoryDiagnoser]
public class WorkListOrderingBenchmarks
{
    private static readonly DateOnly Today = new(2026, 11, 16);

    private List<Todo> _todos = [];

    [Params(100, 1_000, 10_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // Fast seed: samma data varje körning, så att vi mäter samma sak varje gång.
        var random = new Random(42);
        _todos = Enumerable.Range(1, Count)
            .Select(i => new Todo
            {
                Id = i,
                Title = $"Uppgift {i}",
                IsDone = random.Next(4) == 0, // ungefär var fjärde är klar
                DueDate = random.Next(5) == 0 ? null : Today.AddDays(random.Next(-30, 30)), // ungefär var femte saknar datum
            })
            .ToList();
    }

    [Benchmark]
    public List<Todo> SortWorkList() => TodoOrdering.ForWorkList(_todos).ToList();
}
