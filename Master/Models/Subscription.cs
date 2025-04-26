using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Subscription
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PaymentId { get; set; }

    public string SubscriptionType { get; set; } = null!;

    public string? DayOfWeek { get; set; }

    public TimeOnly Time { get; set; }

    public string Location { get; set; } = null!;

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? MaterialType { get; set; }

    public string? Quantity { get; set; }

    public string? City { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
