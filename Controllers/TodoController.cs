using Microsoft.AspNetCore.Mvc;
using TodoManager.Models;
using Microsoft.EntityFrameworkCore;
using TodoManager.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace TodoManager.Controllers
{
    public class TodoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityModels> _userManager;
        private readonly ILogger<TodoController> _logger;

        public TodoController(AppDbContext context, UserManager<IdentityModels> userManager, ILogger<TodoController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Todo
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var todos = await _context.TodoItems
                .Where(t => t.UserId == userId)
                .ToListAsync();
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
        public async Task<IActionResult> Create(Todo todo)
        {
            todo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                //todo.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                todo.CreatedAt = DateTime.UtcNow;
                todo.UpdatedAt = DateTime.UtcNow;

                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Optional: Log errors
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
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

        // GET: Todo/Delete/5
        public IActionResult Delete(int id)
        {
            var todo = _context.TodoItems.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todo);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
