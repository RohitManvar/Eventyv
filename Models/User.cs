using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EventBriteClone.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEmailVerified { get; set; }

        // Navigation Properties
        public virtual ICollection<Event> OrganizedEvents { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<EventLike> LikedEvents { get; set; }
        public virtual ICollection<Following> Following { get; set; }
        public virtual ICollection<Following> Followers { get; set; }
    }
}
