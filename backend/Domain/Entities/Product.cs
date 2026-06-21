using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Product
{
    public long Id { get; set; }

    public long? CategoryId { get; set; }

    public long? BrandId { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string Slug { get; set; } = null!;

    public string? Sku { get; set; }

    public string? Barcode { get; set; }

    public string? DescriptionEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? ShortDescriptionEn { get; set; }

    public string? ShortDescriptionAr { get; set; }

    public decimal Price { get; set; }

    public decimal? CompareAtPrice { get; set; }

    public decimal? CostPrice { get; set; }

    public string Currency { get; set; } = null!;

    public int StockQuantity { get; set; }

    public int LowStockThreshold { get; set; }

    public int MinOrderQuantity { get; set; }

    public decimal? Weight { get; set; }

    public string Status { get; set; } = null!;

    public bool IsFeatured { get; set; }

    public bool RequiresShipping { get; set; }

    public int WarrantyMonths { get; set; }

    public string? WarrantyDescription { get; set; }

    public int TotalSold { get; set; }

    public int ViewsCount { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public string? Tags { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BackInStockRequest> BackInStockRequests { get; set; } = new List<BackInStockRequest>();

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<CollectionProduct> CollectionProducts { get; set; } = new List<CollectionProduct>();

    public virtual ICollection<FlashSaleProduct> FlashSaleProducts { get; set; } = new List<FlashSaleProduct>();

    public virtual ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();

    public virtual ICollection<ProductCompare> ProductCompares { get; set; } = new List<ProductCompare>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
