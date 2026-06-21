using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class MenuItem
{
    public long Id { get; set; }

    public long MenuId { get; set; }

    public long? ParentId { get; set; }

    public string? LabelEn { get; set; }

    public string? LabelAr { get; set; }

    public string? Url { get; set; }

    /// <summary>
    /// custom, category, collection, page, product
    /// </summary>
    public string? LinkType { get; set; }

    public long? LinkId { get; set; }

    public string? Icon { get; set; }

    public string Target { get; set; } = null!;

    public int SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MenuItem> InverseParent { get; set; } = new List<MenuItem>();

    public virtual Menu Menu { get; set; } = null!;

    public virtual MenuItem? Parent { get; set; }
}
