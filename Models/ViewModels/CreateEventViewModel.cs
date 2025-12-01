using EventBriteClone.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models.ViewModels
{
    public class CreateEventViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int CategoryId { get; set; }
        public string EventType { get; set; }
        public string Subcategory { get; set; }
        public string Tags { get; set; }
        public List<TicketViewModel> Tickets { get; set; } = new List<TicketViewModel>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    public class TicketViewModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public enum TicketType
    {
        Free,
        Paid,
        Donation
    }
}
