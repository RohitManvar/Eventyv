namespace EventBriteClone.Models.ViewModels
{
    public class EventSearchViewModel
    {
        public string SearchQuery { get; set; }
        public string Location { get; set; }
        public int? CategoryId { get; set; }
        public string EventType { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? IsFree { get; set; }
        public List<Event> Events { get; set; }
        public List<Category> Categories { get; set; }
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
