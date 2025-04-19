using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Coupon
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public decimal DiscountPercentage { get; set; }

    public DateTime? ExpiryDate { get; set; }

    public bool IsActive { get; set; }
}
