using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Supplier
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? CompanyName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Whatsapp { get; set; }

    public string? Country { get; set; }

    public string? City { get; set; }

    public string? Address { get; set; }

    public string? PaymentTerms { get; set; }

    public string Currency { get; set; } = null!;

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
