using EventBriteClone.Data;
using EventBriteClone.Models;
using EventBriteClone.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;

namespace EventBriteClone.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventsController> _logger;
        private readonly IWebHostEnvironment _environment;

        public EventsController(ApplicationDbContext context,
                                ILogger<EventsController> logger,
                                IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        // GET: /Events
        public async Task<IActionResult> Index(string search, string location, int? categoryId, int page = 1)
        {
            var query = _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .Where(e => e.IsPublished && e.StartDate >= DateTime.Now);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(e => e.Title.Contains(search) || e.Description.Contains(search));

            if (!string.IsNullOrEmpty(location))
                query = query.Where(e => e.City.Contains(location) || e.Location.Contains(location));

            if (categoryId.HasValue)
                query = query.Where(e => e.CategoryId == categoryId.Value);

            int pageSize = 20;
            int totalResults = await query.CountAsync();

            var events = await query
                .OrderBy(e => e.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new EventSearchViewModel
            {
                SearchQuery = search,
                Location = location ?? "Vadodara",
                CategoryId = categoryId,
                Events = events,
                Categories = await _context.Categories.ToListAsync(),
                TotalResults = totalResults,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalResults / (double)pageSize)
            };

            return View(viewModel);
        }

        // GET: /Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var evt = await _context.Events
                .Include(e => e.Organizer)
                .Include(e => e.Category)
                .Include(e => e.Tickets)
                .Include(e => e.Likes)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evt == null) return NotFound();

            var userId = GetCurrentUserId();
            bool isLiked = userId.HasValue &&
                           await _context.EventLikes.AnyAsync(el => el.EventId == id && el.UserId == userId.Value);

            var similarEvents = await _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.CategoryId == evt.CategoryId && e.Id != id && e.IsPublished)
                .OrderBy(e => e.StartDate)
                .Take(4)
                .ToListAsync();

            var viewModel = new EventDetailsViewModel
            {
                Event = evt,
                Organizer = evt.Organizer,
                Tickets = evt.Tickets.ToList(),
                IsLiked = isLiked,
                LikesCount = evt.Likes.Count,
                SimilarEvents = similarEvents
            };

            return View(viewModel);
        }

        // GET: /Events/Create
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("GET /Events/Create called by {User}", User?.Identity?.Name ?? "anonymous");

            var viewModel = new CreateEventViewModel
            {
                Categories = await _context.Categories.ToListAsync()
            };

            _logger.LogInformation("Returning Create view with {CategoryCount} categories", viewModel.Categories?.Count ?? 0);
            return View(viewModel);
        }

        // New: GET: /Events/FindEvent
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FindEvent()
        {
            // Provide categories and a small events set for server-side rendering if wanted.
            var vm = new EventSearchViewModel
            {
                Categories = await _context.Categories.ToListAsync(),
                Events = await _context.Events
                    .Where(e => e.IsPublished && e.StartDate >= DateTime.Now)
                    .Include(e => e.Organizer)
                    .OrderBy(e => e.StartDate)
                    .Take(50)
                    .ToListAsync()
            };

            return View(vm); // returns Views/Events/FindEvent.cshtml
        }

        // POST: /Events/PublishFullEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishFullEvent(CreateEventViewModel model, IFormFile ImageFile)
        {
            if (!IsUserLoggedIn()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                return View("Create", model);
            }

            try
            {
                // Handle Image Upload
                string imageUrl = null;
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "events");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = "/uploads/events/" + fileName;
                }

                // Create Event
                var evt = new Event
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = imageUrl,
                    Location = model.Location,
                    Address = model.Address,
                    City = model.City,
                    State = model.State,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    CategoryId = model.CategoryId,
                    EventType = model.EventType,
                    Subcategory = model.Subcategory,
                    Tags = model.Tags,
                    OrganizerId = GetCurrentUserId().Value,
                    IsPublished = true,
                    CreatedAt = DateTime.Now
                };

                _context.Events.Add(evt);
                await _context.SaveChangesAsync();

                // Add Tickets
                if (model.Tickets != null && model.Tickets.Any())
                {
                    foreach (var t in model.Tickets)
                    {
                        var ticket = new Ticket
                        {
                            EventId = evt.Id,
                            Name = t.Name,
                            Price = t.Price,
                            Quantity = t.Quantity,
                            QuantitySold = 0,
                            IsFree = t.Price <= 0
                        };
                        _context.Tickets.Add(ticket);
                    }
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"Event published: {evt.Title} (ID: {evt.Id})");
                TempData["Success"] = "Your event has been published successfully!";
                return RedirectToAction("Details", new { id = evt.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing full event");
                ModelState.AddModelError("", "An error occurred while publishing the event.");
                model.Categories = await _context.Categories.ToListAsync();
                return View("Create", model);
            }
        }

        // Optional lightweight API endpoints used by FindEvent client-side JS
        [HttpGet("/api/events")]
        public async Task<IActionResult> ApiGetEvents()
        {
            var events = await _context.Events
                .Where(e => e.IsPublished && e.StartDate >= DateTime.Now)
                .Include(e => e.Organizer)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            var payload = events.Select(e => new
            {
                id = e.Id,
                title = e.Title,
                description = e.Description,
                imageUrl = e.ImageUrl,
                location = e.Location,
                city = e.City,
                startDate = e.StartDate,
                price = e.Price,
                isFree = e.IsFree,
                categoryId = e.CategoryId,
                organizer = new { firstName = e.Organizer?.FirstName, lastName = e.Organizer?.LastName }
            });

            return Ok(payload);
        }

        [HttpGet("/api/categories")]
        public async Task<IActionResult> ApiGetCategories()
        {
            var cats = await _context.Categories
                .Select(c => new { id = c.Id, name = c.Name })
                .ToListAsync();
            return Ok(cats);
        }

        // Helper methods
        private bool IsUserLoggedIn() => User.Identity?.IsAuthenticated ?? false;

        private int? GetCurrentUserId()
        {
            if (!IsUserLoggedIn()) return null;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId) ? userId : null;
        }
    }
}