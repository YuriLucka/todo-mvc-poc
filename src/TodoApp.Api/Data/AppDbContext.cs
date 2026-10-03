using Microsoft.EntityFrameworkCore;
using TodoApp.Shared;

namespace TodoApp.Api.Data;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public TodoDto ToDto() => new(Id, Title, IsDone, CreatedAt);
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(e =>
        {
            e.ToTable("todos");
            e.Property(t => t.Title).HasMaxLength(TodoInput.TitleMaxLength).IsRequired();
        });
    }
}
