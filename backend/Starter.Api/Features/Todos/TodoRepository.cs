using Microsoft.EntityFrameworkCore;
using Starter.Api.Data;
using Starter.Api.Domain;

namespace Starter.Api.Features.Todos;

public sealed class TodoRepository(AppDbContext db) : ITodoRepository
{
    public async Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking().OrderByDescending(t => t.Id).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking()
            .Where(TodoDeadlines.IsOverdueOn(today))
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<CompletedDeadline>> ListCompletedDeadlinesAsync(CancellationToken cancellationToken) =>
        await db.Todos.AsNoTracking()
            .Where(t => t.IsDone && t.DueDate != null && t.CompletedAt != null)
            .Select(t => new CompletedDeadline(t.DueDate!.Value, t.CompletedAt!.Value))
            .ToListAsync(cancellationToken);

    public Task<int> CountOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        db.Todos.CountAsync(TodoDeadlines.IsOverdueOn(today), cancellationToken);

    public Task<Todo?> FindAsync(int id, CancellationToken cancellationToken) =>
        db.Todos.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task AddAsync(Todo todo, CancellationToken cancellationToken) =>
        await db.Todos.AddAsync(todo, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
