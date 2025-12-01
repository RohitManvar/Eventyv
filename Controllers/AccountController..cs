using EventBriteClone.Data;
using EventBriteClone.Models;
using EventBriteClone.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EventBriteClone.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already logged in via session
            if (HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLower();
            var passwordHash = HashPassword(model.Password);

            // Fetch user from database
            var user = _context.Users
                .AsEnumerable() // needed to safely use ToLower
                .FirstOrDefault(u => u.Email != null && u.PasswordHash != null &&
                                     u.Email.ToLower() == email && u.PasswordHash == passwordHash);

            if (user != null)
            {
                // Set session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("Index", "Home");
            }

            ViewData["Error"] = "Invalid email or password";
            return View(model);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLower();

            if (_context.Users.Any(u => u.Email != null && u.Email.ToLower() == email))
            {
                ViewData["Error"] = "An account with this email already exists";
                return View(model);
            }

            var user = new User
            {
                Email = email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PasswordHash = HashPassword(model.Password),
                CreatedAt = DateTime.Now,
                IsEmailVerified = false
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Auto-login after registration
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserEmail", user.Email);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login");

            var user = _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Ticket)
                            .ThenInclude(t => t.Event)
                .Include(u => u.LikedEvents)
                    .ThenInclude(le => le.Event)
                .Include(u => u.Following)
                .FirstOrDefault(u => u.Id == userId.Value);

            if (user == null) return NotFound();

            var upcomingOrders = user.Orders
                .Where(o => o.OrderItems.Any(oi => oi.Ticket.Event.StartDate >= DateTime.Now))
                .OrderBy(o => o.OrderDate)
                .ToList();

            var pastOrders = user.Orders
                .Where(o => o.OrderItems.All(oi => oi.Ticket.Event.StartDate < DateTime.Now))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var viewModel = new UserProfileViewModel
            {
                User = user,
                OrdersCount = user.Orders.Count,
                LikesCount = user.LikedEvents.Count,
                FollowingCount = user.Following.Count,
                UpcomingOrders = upcomingOrders,
                PastOrders = pastOrders
            };

            return View(viewModel);
        }

        // Helper: hash password using SHA256
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
