using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class DeliveryAgent
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Whatsapp { get; set; }

    public string? NationalId { get; set; }

    public string? Area { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<CashCollection> CashCollections { get; set; } = new List<CashCollection>();

    public virtual ICollection<OrderDelivery> OrderDeliveries { get; set; } = new List<OrderDelivery>();

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
}
