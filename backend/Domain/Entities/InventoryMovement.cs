using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class InventoryMovement
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public long? VariantId { get; set; }

    /// <summary>
    /// PURCHASE_IN, SALE_OUT, RETURN_IN, ADJUSTMENT_IN, ADJUSTMENT_OUT, DAMAGE_OUT
    /// </summary>
    public string MovementType { get; set; } = null!;

    public int Quantity { get; set; }

    public int QuantityBefore { get; set; }

    public int QuantityAfter { get; set; }

    /// <summary>
    /// purchase_order, order, return, manual
    /// </summary>
    public string? ReferenceType { get; set; }

    public long? ReferenceId { get; set; }

    public decimal? UnitCost { get; set; }

    public string? Notes { get; set; }

    public long? AdminUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AdminUser? AdminUser { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
