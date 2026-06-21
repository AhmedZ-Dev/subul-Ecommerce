using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Collection
{
    public long Id { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string Slug { get; set; } = null!;

    public string? DescriptionEn { get; set; }

    public string? DescriptionAr { get; set; }

    public string? ImageUrl { get; set; }

    public string? BannerUrl { get; set; }

    public string CollectionType { get; set; } = null!;

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CollectionProduct> CollectionProducts { get; set; } = new List<CollectionProduct>();
}
