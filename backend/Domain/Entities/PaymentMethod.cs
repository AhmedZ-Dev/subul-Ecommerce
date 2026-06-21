using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class PaymentMethod
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LabelEn { get; set; }

    public string? LabelAr { get; set; }

    /// <summary>
    /// offline, online
    /// </summary>
    public string? Type { get; set; }

    public string? Gateway { get; set; }

    public string? GatewayConfig { get; set; }

    public string? IconUrl { get; set; }

    public string? InstructionsEn { get; set; }

    public string? InstructionsAr { get; set; }

    public bool IsActive { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
