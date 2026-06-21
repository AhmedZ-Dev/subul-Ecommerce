using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class User
{
    public long Id { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public bool AcceptsMarketing { get; set; }

    public bool IsActive { get; set; }

    public DateTime? EmailVerifiedAt { get; set; }

    /// <summary>
    /// رصيد المتجر للاسترداد
    /// </summary>
    public decimal StoreCredit { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<BackInStockRequest> BackInStockRequests { get; set; } = new List<BackInStockRequest>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();

    public virtual ICollection<DiscountUsage> DiscountUsages { get; set; } = new List<DiscountUsage>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ProductCompare> ProductCompares { get; set; } = new List<ProductCompare>();

    public virtual ICollection<Return> Returns { get; set; } = new List<Return>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WarrantyClaim> WarrantyClaims { get; set; } = new List<WarrantyClaim>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
