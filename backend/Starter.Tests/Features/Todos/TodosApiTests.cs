using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Starter.Api.Features.Todos;

namespace Starter.Tests.Features.Todos;

/// <summary>Starts the real API in memory, with its own throwaway SQLite file and a fake clock, and talks to it over HTTP.</summary>
public sealed class TodosApiTests : IClassFixture<TodosApiTests.ApiFactory>
{
    /// <summary>Måndag 2026-11-16 kl. 09:00 i Sverige (vintertid, UTC+1).</summary>
    private static readonly DateTimeOffset MondayNineSwedishTime = new(2026, 11, 16, 8, 0, 0, TimeSpan.Zero);

    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public TodosApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_then_get_returns_the_created_todo()
    {
        var created = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Write a test"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var todos = await _client.GetFromJsonAsync<List<TodoResponse>>("/api/todos");

        Assert.Contains(todos!, t => t.Title == "Write a test");
    }

    [Fact]
    public async Task Post_with_empty_title_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest(""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Krav", "AK-1.1")]
    public async Task Post_with_a_due_date_returns_201_and_a_date_without_time()
    {
        _factory.Clock.SetUtcNow(MondayNineSwedishTime);

        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Boka möte", new DateOnly(2026, 11, 20)));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Contains("\"dueDate\":\"2026-11-20\"", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    [Trait("Krav", "AK-1.2")]
    public async Task Post_with_a_due_date_that_has_passed_returns_400()
    {
        _factory.Clock.SetUtcNow(MondayNineSwedishTime);

        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Skicka offert", new DateOnly(2026, 11, 15)));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("Krav", "AK-2.1")]
    public async Task A_todo_due_today_is_not_overdue_at_nine_in_the_morning()
    {
        _factory.Clock.SetUtcNow(MondayNineSwedishTime);
        var created = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Skicka offert", new DateOnly(2026, 11, 16)));
        var offer = await created.Content.ReadFromJsonAsync<TodoResponse>();

        var overdue = await _client.GetFromJsonAsync<List<OverdueTodoResponse>>("/api/todos/overdue");

        Assert.DoesNotContain(overdue!, t => t.Id == offer!.Id);
    }

    public sealed class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"starter-test-{Guid.NewGuid()}.db");

        public FakeTimeProvider Clock { get; } = new(MondayNineSwedishTime);

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ConnectionStrings:Default", $"Data Source={_databasePath}");
            builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(Clock));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
            File.Delete(_databasePath);
        }
    }
}
