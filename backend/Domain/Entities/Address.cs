using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Address
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Phone { get; set; }

    public string Address1 { get; set; } = null!;

    public string? Address2 { get; set; }

    public string? City { get; set; }

    public string? Governorate { get; set; }

    public string Country { get; set; } = null!;

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
