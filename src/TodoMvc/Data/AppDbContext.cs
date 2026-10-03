using Microsoft.EntityFrameworkCore;
using TodoMvc.Models;

namespace TodoMvc.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(e =>
        {
            e.ToTable("todos");
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
        });
    }
}
