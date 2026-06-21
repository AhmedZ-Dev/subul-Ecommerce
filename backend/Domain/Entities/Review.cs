using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Review
{
    public long Id { get; set; }

    public long ProductId { get; set; }

    public long UserId { get; set; }

    public long? OrderItemId { get; set; }

    public short Rating { get; set; }

    public string? Title { get; set; }

    public string? Body { get; set; }

    public string Status { get; set; } = null!;

    public bool IsVerifiedPurchase { get; set; }

    public int HelpfulCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual OrderItem? OrderItem { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
