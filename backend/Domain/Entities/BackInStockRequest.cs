using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class BackInStockRequest
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string? Email { get; set; }

    public long ProductId { get; set; }

    public long? VariantId { get; set; }

    public DateTime? NotifiedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual ProductVariant? Variant { get; set; }
}
