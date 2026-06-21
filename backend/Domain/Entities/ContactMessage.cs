using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ContactMessage
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Subject { get; set; }

    public string Message { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? RepliedAt { get; set; }

    public long? RepliedBy { get; set; }

    public string? ReplyMessage { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AdminUser? RepliedByNavigation { get; set; }

    public virtual User? User { get; set; }
}
