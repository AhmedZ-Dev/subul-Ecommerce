using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class OrderItem
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long? ProductId { get; set; }

    public long? VariantId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Sku { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalPrice { get; set; }

    public int WarrantyMonths { get; set; }

    public bool RequiresShipping { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ProductVariant? Variant { get; set; }

    public virtual ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
}
