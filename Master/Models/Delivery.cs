using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Delivery
{
    public int Id { get; set; }

    public int? PaymentId { get; set; }

    public int? RequestId { get; set; }

    public string ReceiverName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Location { get; set; } = null!;

    public DateTime DeliveryTime { get; set; }

    public bool? PaidDelivery { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual RecyclingRequest? Request { get; set; }
}
