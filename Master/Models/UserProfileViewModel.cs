﻿namespace Master.Models
{
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public List<Order> Orders { get; set; }
        public List<RecyclingRequest> RecyclingRequests { get; set; }
        public bool IsOwner { get; set; }
        public List<Company> Companies { get; set; }

        // إضافة خصائص محسوبة
        public int TotalRecycledItems => RecyclingRequests?.Sum(r => r.Quantity) ?? 0;
        public double TotalCO2Saved => RecyclingRequests?.Sum(r => r.Quantity * 1.75) ?? 0;
        public int TotalOrders => Orders?.Count ?? 0;
    }
}
