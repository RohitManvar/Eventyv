using EventBriteClone.Models;
using EventBriteClone.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventBriteClone.Data
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, string location)
        {
            var viewModel = new HomeViewModel
            {
                SearchQuery = search,
                Location = location ?? "Vadodara",
                Categories = _context.Categories.ToList(),
                FeaturedEvents = _context.Events
                    .Include(e => e.Organizer)
                    .Include(e => e.Category)
                    .Where(e => e.IsPublished && e.StartDate >= DateTime.Now)
                    .OrderBy(e => e.StartDate)
                    .Take(12)
                    .ToList(),
                PopularCities = new List<string> { "New York", "Los Angeles", "Chicago", "Washington" }
            };

            if (!string.IsNullOrEmpty(search))
            {
                viewModel.FeaturedEvents = viewModel.FeaturedEvents
                    .Where(e => e.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                               e.Description.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(viewModel);
        }
    }
}
