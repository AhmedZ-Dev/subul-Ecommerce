using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Attribute
{
    public long Id { get; set; }

    public long? GroupId { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string? Slug { get; set; }

    public string? Unit { get; set; }

    public string InputType { get; set; } = null!;

    public bool IsFilterable { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AttributeGroup? Group { get; set; }

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = new List<ProductAttributeValue>();
}
