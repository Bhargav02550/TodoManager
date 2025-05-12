using Microsoft.AspNetCore.Mvc;
using TodoManager.Data;
using TodoManager.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TodoManager.Controllers
{
    public class EditController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EditController> _logger;

        public EditController(AppDbContext context, ILogger<EditController> logger)
        {
            _context = context;
            _logger = logger;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return NotFound();
            }
            var existing = await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null) return NotFound();

            todo.CreatedAt = existing.CreatedAt;

            if (ModelState.IsValid)
            {
                try
                {
                    todo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    todo.UpdatedAt = DateTime.UtcNow;

                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Todo");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TodoItems.Any(e => e.Id == id)) // Corrected property name to 'TodoItems'
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            return View(todo);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Todo");
        }
    }
}
