using System;
using System.Collections.Generic;

namespace Master.Models;

public partial class Contact
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public bool IsRead { get; set; }

    public string? ReplyMessage { get; set; }

    public DateTime? ReplyDate { get; set; }
}
