namespace EventBriteClone.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public int OrdersCount { get; set; }
        public int LikesCount { get; set; }
        public int FollowingCount { get; set; }
        public List<Order> UpcomingOrders { get; set; }
        public List<Order> PastOrders { get; set; }
    }
}
