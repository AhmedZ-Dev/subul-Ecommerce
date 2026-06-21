using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class OrderDelivery
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long DeliveryAgentId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public long? AssignedBy { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? PickedUpAt { get; set; }

    public DateTime? AttemptedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public string? FailureReason { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AdminUser? AssignedByNavigation { get; set; }

    public virtual DeliveryAgent DeliveryAgent { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
