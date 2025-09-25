using Eventyv.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Eventyv.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventyvContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventsController(EventyvContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Events/ExploreEvent
        public IActionResult ExploreEvent()
        {
            var events = _context.Events
                                 .OrderBy(e => e.StartDate)
                                 .ToList();
            return View(events);
        }

        // GET: /Events/CreateEvent
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        // POST: /Events/CreateEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(Event model)
        {
            if (ModelState.IsValid)
            {
                string imagePath = null;

                if (model.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    imagePath = "/uploads/" + uniqueFileName;
                }

                var newEvent = new Event
                {
                    Title = model.Title,
                    Description = model.Description,
                    Category = model.Category,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    IsOnline = model.IsOnline,
                    Location = model.Location,
                    Address = model.Address,
                    IsFree = model.IsFree,
                    Price = model.IsFree ? 0 : model.Price,
                    MaxAttendees = model.MaxAttendees,
                    ImagePath = imagePath,
                    Status = EventStatus.Active,
                    CreatedAt = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                return RedirectToAction("ExploreEvent");
            }

            return View(model);
        }

        // GET: /Events/Search?q=term
        public IActionResult Search(string q)
        {
            var events = string.IsNullOrEmpty(q)
                ? _context.Events.OrderBy(e => e.StartDate).ToList()
                : _context.Events.Where(e => e.Title.Contains(q, StringComparison.OrdinalIgnoreCase))
                                 .OrderBy(e => e.StartDate)
                                 .ToList();

            ViewBag.SearchQuery = q;
            return View("ExploreEvent", events);
        }

        // GET: /Events/ByCategory?category=music
        public IActionResult ByCategory(string category)
        {
            var events = string.IsNullOrEmpty(category) || category.ToLower() == "all"
                ? _context.Events.OrderBy(e => e.StartDate).ToList()
                : _context.Events.Where(e => e.Category.ToLower() == category.ToLower())
                                 .OrderBy(e => e.StartDate)
                                 .ToList();

            ViewBag.SelectedCategory = category;
            return View("ExploreEvent", events);
        }

        // GET: /Events/Details/{id}
        public IActionResult Details(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.EventId == id); // use Id property
            if (ev == null) return NotFound();
            return View(ev);
        }
    }
}
