using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class CartItem
{
    public long Id { get; set; }

    public long CartId { get; set; }

    public long ProductId { get; set; }

    public long? VariantId { get; set; }

    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
