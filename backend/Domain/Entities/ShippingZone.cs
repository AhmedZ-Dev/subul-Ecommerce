using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ShippingZone
{
    public long Id { get; set; }

    public string NameEn { get; set; } = null!;

    public string? NameAr { get; set; }

    public string? Governorates { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<ShippingRate> ShippingRates { get; set; } = new List<ShippingRate>();
}
