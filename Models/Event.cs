using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public int Capacity { get; set; }
        public int AvailableTickets { get; set; }

        public string EventType { get; set; } // Tradeshow, Concert, etc.
        public string Subcategory { get; set; }
        public string Tags { get; set; } // Comma-separated

        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Foreign Keys
        public int OrganizerId { get; set; }
        public int CategoryId { get; set; }

        // Navigation Properties
        public virtual User Organizer { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<EventLike> Likes { get; set; }
    }
}