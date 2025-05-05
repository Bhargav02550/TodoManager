using Microsoft.AspNetCore.Mvc;
using TodoManager.Models;
using Microsoft.EntityFrameworkCore;
using TodoManager.Data;

namespace TodoManager.Controllers
{
    public class TodoController : Controller
    {
        private AppDbContext _context;

        public TodoController(AppDbContext context)
        {
            _context = context;
        }
        // GET: Todo
        public async Task<IActionResult> Index()
        {
            var todos = await _context.TodoItems.ToListAsync();
            return View(todos);
        }

        // GET: Todo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Priority,CreatedAt,UpdatedAt")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                todo.CreatedAt = DateTime.UtcNow;
                todo.UpdatedAt = DateTime.UtcNow;
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}