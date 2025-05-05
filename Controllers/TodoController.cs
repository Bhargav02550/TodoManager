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
            else
            {
                // Log the model state errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
                return View(todo);
        }

        // GET: Todo/Edit/5
        public IActionResult Edit(int id)
        {
            var todo = _context.TodoItems.Find(id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}