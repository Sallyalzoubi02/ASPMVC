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

    public virtual Payment? Payment { get; set; }

    public virtual User? User { get; set; }
}
