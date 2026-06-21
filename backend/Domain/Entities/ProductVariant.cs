using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ProductVariant
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public string? Title { get; set; }

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public decimal? Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public int StockQuantity { get; set; }

    public decimal? Weight { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BackInStockRequest> BackInStockRequests { get; set; } = new List<BackInStockRequest>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<FlashSaleProduct> FlashSaleProducts { get; set; } = new List<FlashSaleProduct>();

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();
}
