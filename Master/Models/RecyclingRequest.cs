using System;
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

    public int? PaymentId { get; set; }

    public bool? DeliveryStatus { get; set; }

    public string? Location { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
