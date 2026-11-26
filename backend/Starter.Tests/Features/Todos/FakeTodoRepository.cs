using Starter.Api.Domain;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>An in-memory stand-in for the real repository, so service tests need no database.</summary>
internal sealed class FakeTodoRepository : ITodoRepository
{
    private readonly List<Todo> _todos = [];
    private int _nextId = 1;

    public IReadOnlyList<Todo> Saved => _todos;

    public Task<IReadOnlyList<Todo>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Todo>>(_todos.OrderByDescending(t => t.Id).ToList());

    public Task<IReadOnlyList<Todo>> ListOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<Todo>>(
            _todos.Where(t => TodoDeadlines.IsOverdue(t, today)).OrderBy(t => t.DueDate).ThenBy(t => t.Id).ToList());

    public Task<IReadOnlyList<CompletedDeadline>> ListCompletedDeadlinesAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<CompletedDeadline>>(
            _todos.Where(t => t.IsDone && t.DueDate != null && t.CompletedAt != null)
                .Select(t => new CompletedDeadline(t.DueDate!.Value, t.CompletedAt!.Value))
                .ToList());

    public Task<int> CountOverdueAsync(DateOnly today, CancellationToken cancellationToken) =>
        Task.FromResult(_todos.Count(t => TodoDeadlines.IsOverdue(t, today)));

    public Task<Todo?> FindAsync(int id, CancellationToken cancellationToken) =>
        Task.FromResult(_todos.FirstOrDefault(t => t.Id == id));

    public Task AddAsync(Todo todo, CancellationToken cancellationToken)
    {
        todo.Id = _nextId++;
        _todos.Add(todo);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
