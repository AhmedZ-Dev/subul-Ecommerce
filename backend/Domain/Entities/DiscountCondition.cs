using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class DiscountCondition
{
    public long Id { get; set; }

    public long DiscountId { get; set; }

    public string? EntityType { get; set; }

    public long? EntityId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Discount Discount { get; set; } = null!;
}
