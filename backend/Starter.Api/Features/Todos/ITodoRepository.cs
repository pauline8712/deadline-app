using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

/// <summary>Data access for todos. The service depends on this interface, which makes it testable without a database.</summary>
public interface ITodoRepository
{
    Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Ej klara uppgifter som är försenade på <paramref name="today"/>, tidigast förfallodatum först. Filtreras i databasen.</summary>
    Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken);

    Task<Todo?> FindAsync(int id, CancellationToken cancellationToken);

    Task AddAsync(Todo todo, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
