using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Return
{
    public long Id { get; set; }

    public string ReturnNumber { get; set; } = null!;

    public long OrderId { get; set; }

    public long UserId { get; set; }

    /// <summary>
    /// return, exchange, warranty_repair
    /// </summary>
    public string? ReturnType { get; set; }

    public string Status { get; set; } = null!;

    public string? Reason { get; set; }

    public string? ReasonDetails { get; set; }

    public decimal? RefundAmount { get; set; }

    /// <summary>
    /// original_payment, store_credit, bank_transfer
    /// </summary>
    public string? RefundMethod { get; set; }

    public string RefundStatus { get; set; } = null!;

    public string? AdminNotes { get; set; }

    public long? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual ICollection<ReturnItem> ReturnItems { get; set; } = new List<ReturnItem>();

    public virtual AdminUser? ReviewedByNavigation { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();
}
