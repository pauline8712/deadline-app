using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;
using Starter.Api.Shared;

namespace Starter.Tests.Features.Todos;

public sealed class TodoServiceTests
{
    /// <summary>Måndag 2026-11-16 kl. 10:00 i Sverige (vintertid, UTC+1).</summary>
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 11, 16, 9, 0, 0, TimeSpan.Zero));
    private readonly FakeTodoRepository _repository = new();
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _service = new TodoService(_repository, _clock);
    }

    [Fact]
    [Trait("Krav", "AK-1.2")]
    public async Task Create_returns_validation_error_when_due_date_has_passed()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("Skicka offert", new DateOnly(2026, 11, 15)), CancellationToken.None);

        Assert.Equal(ErrorKind.Validation, result.Kind);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task Create_trims_title_and_saves_the_todo()
    {
        var result = await _service.CreateAsync(new CreateTodoRequest("  Buy milk  "), CancellationToken.None);

        Assert.True(result.Success);
        Assert.Equal("Buy milk", result.Data!.Title);
        Assert.Single(_repository.Saved);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_returns_validation_error_when_title_is_empty(string title)
    {
        var result = await _service.CreateAsync(new CreateTodoRequest(title), CancellationToken.None);

        Assert.False(result.Success);
        Assert.Equal(ErrorKind.Validation, result.Kind);
        Assert.Empty(_repository.Saved);
    }

    [Fact]
    public async Task Create_returns_validation_error_when_title_is_too_long()
    {
        var tooLong = new string('x', TodoRules.MaxTitleLength + 1);

        var result = await _service.CreateAsync(new CreateTodoRequest(tooLong), CancellationToken.None);

        Assert.Equal(ErrorKind.Validation, result.Kind);
    }

    [Fact]
    public async Task MarkDone_returns_not_found_for_unknown_id()
    {
        var result = await _service.MarkDoneAsync(42, CancellationToken.None);

        Assert.Equal(ErrorKind.NotFound, result.Kind);
    }
}
