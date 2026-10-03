using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoMvc.Data;
using TodoMvc.Models;

namespace TodoMvc.Controllers;

public class TodosController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var todos = await db.Todos
            .AsNoTracking()
            .OrderBy(t => t.IsDone)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();
        return View(todos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(nameof(TodoItem.Title))] TodoItem item)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        db.Todos.Add(new TodoItem { Title = item.Title.Trim() });
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var item = await db.Todos.FindAsync(id);
        if (item is null) return NotFound();

        item.IsDone = !item.IsDone;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Todos.FindAsync(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind(nameof(TodoItem.Title), nameof(TodoItem.IsDone))] TodoItem input)
    {
        var item = await db.Todos.FindAsync(id);
        if (item is null) return NotFound();
        if (!ModelState.IsValid) return View(input);

        item.Title = input.Title.Trim();
        item.IsDone = input.IsDone;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await db.Todos.Where(t => t.Id == id).ExecuteDeleteAsync();
        return RedirectToAction(nameof(Index));
    }
}
