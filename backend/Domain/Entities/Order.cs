using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Order
{
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public string FulfillmentStatus { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ShippingAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal Total { get; set; }

    public string Currency { get; set; } = null!;

    public long? DiscountId { get; set; }

    public string? CouponCode { get; set; }

    public string? ShippingFirstName { get; set; }

    public string? ShippingLastName { get; set; }

    public string? ShippingPhone { get; set; }

    public string? ShippingAddress1 { get; set; }

    public string? ShippingAddress2 { get; set; }

    public string? ShippingCity { get; set; }

    public string? ShippingGovernorate { get; set; }

    public string ShippingCountry { get; set; } = null!;

    public long? ShippingZoneId { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TrackingNumber { get; set; }

    public string? Notes { get; set; }

    public string? CustomerNotes { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancelReason { get; set; }

    public DateTime? ShippedAt { get; set; }

    public DateTime? DeliveredAt { get; set; }

    public string? IpAddress { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Discount? Discount { get; set; }

    public virtual ICollection<DiscountUsage> DiscountUsages { get; set; } = new List<DiscountUsage>();

    public virtual ICollection<OrderDelivery> OrderDeliveries { get; set; } = new List<OrderDelivery>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<Return> Returns { get; set; } = new List<Return>();

    public virtual ShippingZone? ShippingZone { get; set; }

    public virtual User? User { get; set; }
}
