using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Cart
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string? SessionId { get; set; }

    public string? CouponCode { get; set; }

    public string? Notes { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
