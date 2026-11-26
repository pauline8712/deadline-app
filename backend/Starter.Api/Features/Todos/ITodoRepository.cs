using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>Data access for todos. The service depends on this interface, which makes it testable without a database.</summary>
public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Ej klara uppgifter som är försenade på <paramref name="today"/>, tidigast förfallodatum först. Filtreras i databasen.</summary>
    Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken);

    /// <summary>Förfallodatum och klar-tidpunkt för alla klara uppgifter som har ett förfallodatum (US-4).</summary>
    Task<IReadOnlyList<CompletedDeadline>> ListCompletedDeadlinesAsync(CancellationToken cancellationToken);

    /// <summary>Antal ej klara uppgifter som är försenade på <paramref name="today"/>. Räknas i databasen.</summary>
    Task<int> CountOverdueAsync(DateOnly today, CancellationToken cancellationToken);

    Task<Todo?> FindAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Todo todo, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
