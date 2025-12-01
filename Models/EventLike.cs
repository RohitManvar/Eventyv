namespace EventBriteClone.Models
{
    public class EventLike
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime LikedAt { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual Event Event { get; set; }
    }
}
