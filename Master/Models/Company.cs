using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Company
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public int OwnerId { get; set; }

    public virtual User Owner { get; set; } = null!;
}
