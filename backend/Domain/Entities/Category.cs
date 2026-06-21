using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Category
{
    public long Id { get; set; }

    public long? ParentId { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string Slug { get; set; } = null!;

    public string? DescriptionEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? ImageUrl { get; set; }

    public string? BannerUrl { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public string? SeoTitle { get; set; }

    public string? SeoDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
