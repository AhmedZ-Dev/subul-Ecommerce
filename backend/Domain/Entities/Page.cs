using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Page
{
    public long Id { get; set; }

    public string TitleEn { get; set; } = null!;

    public string? TitleAr { get; set; }

    public string Slug { get; set; } = null!;

    public string? ContentEn { get; set; }

    public string? ContentAr { get; set; }

    public bool IsPublished { get; set; }

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
