using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? City { get; set; }

    public string Location { get; set; } = null!;

    public string UserType { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string? Image { get; set; }

    public int? Points { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<RecyclingRequest> RecyclingRequests { get; set; } = new List<RecyclingRequest>();

    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
