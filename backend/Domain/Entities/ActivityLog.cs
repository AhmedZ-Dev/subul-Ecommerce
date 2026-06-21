using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ActivityLog
{
    public long Id { get; set; }

    public long? AdminUserId { get; set; }

    public string Action { get; set; } = null!;

    public string? EntityType { get; set; }

    public long? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AdminUser? AdminUser { get; set; }
}
