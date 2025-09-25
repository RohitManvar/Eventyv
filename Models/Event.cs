using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Eventyv.Models
{
    public enum EventStatus
    {
        Upcoming = 0,
        Active = 1,
        Completed = 2,
        Cancelled = 3
    }

    public class Event
    {
        [Key]
        public int EventId { get; set; }

        // Basic Info
        [Required(ErrorMessage = "Event title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Event description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        // Date & Time
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        // Location
        public bool IsOnline { get; set; }

        [StringLength(150)]
        public string Location { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        // Pricing
        public bool IsFree { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal? Price { get; set; }

        public int? MaxAttendees { get; set; }

        // Image
        public string ImagePath { get; set; } // store file path or URL

        [NotMapped]
        public IFormFile ImageFile { get; set; } // used for file upload only, not stored in DB

        // Status
        [Required]
        public EventStatus Status { get; set; } = EventStatus.Upcoming;

        // Metadata
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; } // optional: link to user who created
    }
}
