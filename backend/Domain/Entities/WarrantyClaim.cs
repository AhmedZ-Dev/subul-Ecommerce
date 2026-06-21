using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class WarrantyClaim
{
    public long Id { get; set; }

    public string ClaimNumber { get; set; } = null!;

    public long OrderItemId { get; set; }

    public long? ProductId { get; set; }

    public long UserId { get; set; }

    public long? ReturnId { get; set; }

    public string IssueDescription { get; set; } = null!;

    public string Status { get; set; } = null!;

    /// <summary>
    /// repair, replacement, refund
    /// </summary>
    public string? Resolution { get; set; }

    public DateOnly? WarrantyExpiresAt { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public string? AdminNotes { get; set; }

    public long? HandledBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AdminUser? HandledByNavigation { get; set; }

    public virtual OrderItem OrderItem { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual Return? Return { get; set; }

    public virtual User User { get; set; } = null!;
}
