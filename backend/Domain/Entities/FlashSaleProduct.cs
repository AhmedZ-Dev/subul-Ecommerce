using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class FlashSaleProduct
{
    public long Id { get; set; }

    public long FlashSaleId { get; set; }

    public long ProductId { get; set; }

    public long? VariantId { get; set; }

    public decimal SalePrice { get; set; }

    public decimal? OriginalPrice { get; set; }

    public int? MaxQuantityPerOrder { get; set; }

    public int? QuantityLimit { get; set; }

    public int QuantitySold { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual FlashSale FlashSale { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual ProductVariant? Variant { get; set; }
}
