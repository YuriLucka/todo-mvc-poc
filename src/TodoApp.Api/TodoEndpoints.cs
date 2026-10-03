using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Shared;

namespace TodoApp.Api;

public static class TodoEndpoints
{
    public static void MapTodoEndpoints(this IEndpointRouteBuilder app)
    {
        var todos = app.MapGroup("/api/todos");

        todos.MapGet("/", async (AppDbContext db) =>
            await db.Todos
                .AsNoTracking()
                .OrderBy(t => t.IsDone)
                .ThenByDescending(t => t.CreatedAt)
                .Select(t => new TodoDto(t.Id, t.Title, t.IsDone, t.CreatedAt))
                .ToListAsync());

        todos.MapGet("/{id:int}", async (int id, AppDbContext db) =>
            await db.Todos.FindAsync(id) is { } item ? Results.Ok(item.ToDto()) : Results.NotFound());

        todos.MapPost("/", async (TodoInput input, AppDbContext db) =>
        {
            if (Validate(input) is { } problem) return problem;

            var item = new TodoItem { Title = input.Title.Trim(), IsDone = input.IsDone };
            db.Todos.Add(item);
            await db.SaveChangesAsync();
            return Results.Created($"/api/todos/{item.Id}", item.ToDto());
        });

        todos.MapPut("/{id:int}", async (int id, TodoInput input, AppDbContext db) =>
        {
            if (Validate(input) is { } problem) return problem;

            var item = await db.Todos.FindAsync(id);
            if (item is null) return Results.NotFound();

            item.Title = input.Title.Trim();
            item.IsDone = input.IsDone;
            await db.SaveChangesAsync();
            return Results.Ok(item.ToDto());
        });

        todos.MapPatch("/{id:int}/toggle", async (int id, AppDbContext db) =>
        {
            var item = await db.Todos.FindAsync(id);
            if (item is null) return Results.NotFound();

            item.IsDone = !item.IsDone;
            await db.SaveChangesAsync();
            return Results.Ok(item.ToDto());
        });

        todos.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
            await db.Todos.Where(t => t.Id == id).ExecuteDeleteAsync() > 0 ? Results.NoContent() : Results.NotFound());
    }

    private static IResult? Validate(TodoInput input)
    {
        input.Title = input.Title?.Trim() ?? string.Empty;
        var results = new List<ValidationResult>();
        if (Validator.TryValidateObject(input, new ValidationContext(input), results, validateAllProperties: true))
            return null;

        return Results.ValidationProblem(results
            .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
            .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage ?? string.Empty).ToArray()));
    }
}
