using Microsoft.AspNetCore.Mvc;
using Eventyv.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;

namespace Eventyv.Controllers
{
    public class AccountController : Controller
    {
        private readonly EventyvContext _context;

        public AccountController(EventyvContext context)
        {
            _context = context;
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register() => View();

        // POST: Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                // Check if username already exists (if provided and different from email)
                if (!string.IsNullOrEmpty(model.UserName) && model.UserName != model.Email)
                {
                    if (_context.Users.Any(u => u.UserName == model.UserName))
                    {
                        ModelState.AddModelError("UserName", "Username already exists");
                        return View(model);
                    }
                }

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    UserName = string.IsNullOrEmpty(model.UserName) ? model.Email : model.UserName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }
            return View(model);
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Use Where + FirstOrDefault to handle nullable UserName properly
                var user = _context.Users
                    .Where(u => u.Email == model.UserName ||
                               (u.UserName != null && u.UserName == model.UserName))
                    .FirstOrDefault();

                if (user != null &&
                    !string.IsNullOrEmpty(user.PasswordHash) &&
                    BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    // Handle nullable fields safely
                    var userName = user.UserName ?? user.Email ?? "User";
                    var userEmail = user.Email ?? string.Empty;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, userName),
                        new Claim(ClaimTypes.Email, userEmail)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid email/username or password");
            }
            return View(model);
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: ForgotPassword
        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            ViewBag.Message = "If an account with this email exists, you will receive a password reset link.";
            return View();
        }
    }
}