using Microsoft.AspNetCore.Identity;
using System;
namespace Eventyv.Models
{
    public class User: IdentityUser
    {
        public string UserName { get; set; }
        public string ProfilePicUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
