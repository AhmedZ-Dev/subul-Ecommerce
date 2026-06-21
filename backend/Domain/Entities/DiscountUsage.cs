using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class DiscountUsage
{
    public long Id { get; set; }

    public long DiscountId { get; set; }

    public long OrderId { get; set; }

    public long? UserId { get; set; }

    public decimal? AmountSaved { get; set; }

    public DateTime UsedAt { get; set; }

    public virtual Discount Discount { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;

    public virtual User? User { get; set; }
}
