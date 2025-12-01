using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EventBriteClone.Models.ViewModels
{
    public class HomeViewModel
    {
        public string SearchQuery { get; set; }
        public string Location { get; set; }
        public List<Category> Categories { get; set; }
        public List<Event> FeaturedEvents { get; set; }
        public List<string> PopularCities { get; set; }
    }
}