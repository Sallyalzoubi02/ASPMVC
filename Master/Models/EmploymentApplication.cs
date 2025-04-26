using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class EmploymentApplication
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string NationalId { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }

    public string City { get; set; } = null!;

    public string District { get; set; } = null!;

    public int? ExperienceYears { get; set; }

    public string Position { get; set; } = null!;

    public string MotivationText { get; set; } = null!;

    public DateTime? ApplicationDate { get; set; }

    public string? Status { get; set; }
}
