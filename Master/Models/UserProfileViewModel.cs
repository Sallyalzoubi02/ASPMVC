namespace Master.Models
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }
        public List<RecyclingRequest> RecyclingRequests { get; set; }
        public bool IsOwner { get; set; }
        public List<Company> Companies { get; set; }

        public int TotalOrders => Orders?.Count ?? 0;
    }
}
