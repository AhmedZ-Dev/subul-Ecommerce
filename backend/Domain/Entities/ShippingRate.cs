using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class ShippingRate
{
    public long Id { get; set; }

    public long ShippingZoneId { get; set; }

    public string? NameEn { get; set; }

    public string? NameAr { get; set; }

    public string RateType { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal? MinOrderValue { get; set; }

    public decimal? MaxOrderValue { get; set; }

    public decimal? FreeShippingThreshold { get; set; }

    public int? EstimatedDaysMin { get; set; }

    public int? EstimatedDaysMax { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ShippingZone ShippingZone { get; set; } = null!;
}
