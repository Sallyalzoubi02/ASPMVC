using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string? CompanyName { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
