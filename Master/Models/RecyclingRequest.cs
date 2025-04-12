using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class RecyclingRequest
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string MaterialType { get; set; } = null!;

    public string Quantity { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual User? User { get; set; }
}
