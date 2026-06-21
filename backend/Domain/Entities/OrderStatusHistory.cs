using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class OrderStatusHistory
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public string? FromStatus { get; set; }

    public string ToStatus { get; set; } = null!;

    /// <summary>
    /// admin, system, customer
    /// </summary>
    public string? ChangedByType { get; set; }

    public long? AdminUserId { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AdminUser? AdminUser { get; set; }

    public virtual Order Order { get; set; } = null!;
}
