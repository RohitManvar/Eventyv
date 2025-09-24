using Microsoft.AspNetCore.Mvc;
using Eventyv.Models;
using System.Linq;

namespace Eventyv.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventyvContext _context;

        public EventsController(EventyvContext context)
        {
            _context = context;
        }

        // GET: /Events/Search
        public IActionResult Search(string q)
        {
            // Fetch events matching the search term
            var events = string.IsNullOrEmpty(q)
                ? _context.Events.ToList()
                : _context.Events.Where(e => e.Title.Contains(q)).ToList();

            ViewBag.SearchQuery = q; // Pass the search term to the view
            return View("SearchEvent", events);
        }

        // GET: /Events/Details/{id}
        public IActionResult Details(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null) return NotFound();
            return View(ev);
        }
    }
}
