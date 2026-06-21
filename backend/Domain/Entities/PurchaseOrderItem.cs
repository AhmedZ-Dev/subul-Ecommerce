using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class PurchaseOrderItem
{
    public long Id { get; set; }

    public long PurchaseOrderId { get; set; }

    public long? ProductId { get; set; }

    public long? VariantId { get; set; }

    public string? Sku { get; set; }

    public string? ProductName { get; set; }

    public int QuantityOrdered { get; set; }

    public int QuantityReceived { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? TotalCost { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
