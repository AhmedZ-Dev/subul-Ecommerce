using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ReturnItem
{
    public long Id { get; set; }

    public long ReturnId { get; set; }

    public long OrderItemId { get; set; }

    public long? ProductId { get; set; }

    public long? VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public decimal? RefundAmount { get; set; }

    /// <summary>
    /// new, good, damaged, defective
    /// </summary>
    public string? Condition { get; set; }

    public bool ReturnToStock { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual Return Return { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
