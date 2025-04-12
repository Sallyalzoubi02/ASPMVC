using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Order
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PaymentId { get; set; }

    public string? OrderStatus { get; set; }

    public string DeliveryAddress { get; set; } = null!;

    public DateTime DeliveryTime { get; set; }

    public decimal TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
