namespace EventBriteClone.Models.ViewModels
{
    public class EventDetailsViewModel
    {
        public Event Event { get; set; }
        public User Organizer { get; set; }
        public List<Ticket> Tickets { get; set; }
        public bool IsLiked { get; set; }
        public int LikesCount { get; set; }
        public List<Event> SimilarEvents { get; set; }
    }
}
