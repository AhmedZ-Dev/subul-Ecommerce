using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Banner
{
    public long Id { get; set; }

    public string? TitleEn { get; set; }

    public string? TitleAr { get; set; }

    public string? SubtitleEn { get; set; }

    public string? SubtitleAr { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? MobileImageUrl { get; set; }

    public string? LinkUrl { get; set; }

    public string? ButtonTextEn { get; set; }

    public string? ButtonTextAr { get; set; }

    public string? Position { get; set; }

    public int SortOrder { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
