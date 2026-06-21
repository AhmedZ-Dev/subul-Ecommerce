using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class PurchaseOrder
{
    public long Id { get; set; }

    public long SupplierId { get; set; }

    public string PoNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateOnly? OrderDate { get; set; }

    public DateOnly? ExpectedDate { get; set; }

    public DateOnly? ReceivedDate { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal? Total { get; set; }

    public string Currency { get; set; } = null!;

    public decimal ExchangeRate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public decimal PaidAmount { get; set; }

    public string? Notes { get; set; }

    public long? AdminUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AdminUser? AdminUser { get; set; }

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

    public virtual Supplier Supplier { get; set; } = null!;
}
