using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class VolunteerApplication
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? NationalId { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public int Age { get; set; }

    public string AvailabilityDays { get; set; } = null!;

    public string? Skills { get; set; }

    public string MotivationText { get; set; } = null!;

    public bool AgreementAccepted { get; set; }

    public DateTime? ApplicationDate { get; set; }

    public string? Status { get; set; }
}
