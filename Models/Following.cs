using System;

namespace EventBriteClone.Models
{
    public class Following
    {
        public int FollowerId { get; set; }
        public int FollowedId { get; set; }
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User Follower { get; set; } = null!;
        public virtual User Followed { get; set; } = null!;
    }
}
