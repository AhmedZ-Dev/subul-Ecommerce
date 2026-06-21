using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Discount
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }

    public string? Type { get; set; }

    public decimal? Value { get; set; }

    public string Currency { get; set; } = null!;

    public decimal? MinOrderValue { get; set; }

    public int? MinQuantity { get; set; }

    public int? UsageLimit { get; set; }

    public int UsageCount { get; set; }

    public int PerCustomerLimit { get; set; }

    public string? AppliesTo { get; set; }

    public DateTime? StartsAt { get; set; }

    public DateTime? EndsAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<DiscountCondition> DiscountConditions { get; set; } = new List<DiscountCondition>();

    public virtual ICollection<DiscountUsage> DiscountUsages { get; set; } = new List<DiscountUsage>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
