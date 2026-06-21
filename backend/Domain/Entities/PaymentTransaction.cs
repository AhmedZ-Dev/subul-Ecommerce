using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class PaymentTransaction
{
    public long Id { get; set; }

    public long OrderId { get; set; }

    public long? PaymentMethodId { get; set; }

    public string? TransactionNumber { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string Status { get; set; } = null!;

    /// <summary>
    /// charge, refund
    /// </summary>
    public string Type { get; set; } = null!;

    public long? CollectedBy { get; set; }

    public DateTime? CollectedAt { get; set; }

    public string? GatewayName { get; set; }

    public string? GatewayTransactionId { get; set; }

    public string? GatewayStatus { get; set; }

    public string? GatewayResponse { get; set; }

    public string? FailureReason { get; set; }

    public DateTime? PaidAt { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual DeliveryAgent? CollectedByNavigation { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual PaymentMethod? PaymentMethod { get; set; }
}
