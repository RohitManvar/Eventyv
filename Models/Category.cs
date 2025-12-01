using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string IconClass { get; set; }
        public string? Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Event> Events { get; set; }
    }
}