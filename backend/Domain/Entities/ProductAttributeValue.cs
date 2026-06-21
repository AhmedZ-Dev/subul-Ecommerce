using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ProductAttributeValue
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public long AttributeId { get; set; }

    public string? ValueText { get; set; }

    public decimal? ValueNumber { get; set; }

    public bool? ValueBoolean { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Attribute Attribute { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
