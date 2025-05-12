using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoManager.Models;


namespace TodoManager.Controllers
{
    public class AccountController : Controller
    {


        private readonly UserManager<IdentityModels> _userManager;
        private readonly SignInManager<IdentityModels> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityModels> userManager,
            SignInManager<IdentityModels> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true) 
            {
                return RedirectToAction("Index", "Todo");
            }
            return View();
        }


        [AllowAnonymous]
        public ActionResult Register()
        {
            if(User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Todo");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Register method called.");
            //Console.WriteLine("Register method called");
            //Console.WriteLine(model);

            if (ModelState.IsValid)
            {
                _logger.LogInformation("User registration started.");

                var user = new IdentityModels
                {
                    UserName = model.UserName,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("", "Home");
                }

                foreach (var error in result.Errors)
                {
                    _logger.LogError("Registration error: {Error}", error.Description);
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                _logger.LogWarning("User registration failed due to invalid model.");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Model error: {ErrorMessage}", modelError.ErrorMessage);
                }
            }

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        //POST: Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }
    }
}