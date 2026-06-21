using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class FlashSale
{
    public long Id { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string? DescriptionEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? BannerUrl { get; set; }

    /// <summary>
    /// percentage, fixed_amount
    /// </summary>
    public string? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public DateTime StartsAt { get; set; }

    public DateTime EndsAt { get; set; }

    public bool IsActive { get; set; }

    public int? MaxUses { get; set; }

    public int UsesCount { get; set; }

    public int SortOrder { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AdminUser? CreatedByNavigation { get; set; }

    public virtual ICollection<FlashSaleProduct> FlashSaleProducts { get; set; } = new List<FlashSaleProduct>();
}
