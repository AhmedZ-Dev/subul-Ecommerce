using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class AttributeGroup
{
    public long Id { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string? Slug { get; set; }

    public int SortOrder { get; set; }

    public bool IsFilterable { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Attribute> Attributes { get; set; } = new List<Attribute>();
}
