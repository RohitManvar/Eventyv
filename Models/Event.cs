using System;

namespace Eventyv.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }

        public string Status { get; set; }
        public string Organizer { get; set; }
        public decimal Price { get; set; }
        public string Tags { get; set; }
    }
}
