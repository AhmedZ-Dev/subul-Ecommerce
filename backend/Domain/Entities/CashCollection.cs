using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class CashCollection
{
    public long Id { get; set; }

    public long DeliveryAgentId { get; set; }

    public DateOnly CollectionDate { get; set; }

    public decimal? ExpectedAmount { get; set; }

    public decimal? CollectedAmount { get; set; }

    public decimal? Difference { get; set; }

    public string Status { get; set; } = null!;

    public long? ReceivedBy { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual DeliveryAgent DeliveryAgent { get; set; } = null!;

    public virtual AdminUser? ReceivedByNavigation { get; set; }
}
