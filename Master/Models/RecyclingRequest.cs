﻿using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class RecyclingRequest
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string MaterialName { get; set; } = null!;

    public string MaterialType { get; set; } = null!;

    public int Quantity { get; set; }

    public string? ImageUrl { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? ItemCondition { get; set; }

    public string? City { get; set; }

    public DateTime RequestedDate { get; set; }

    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    public virtual User? User { get; set; }
}
