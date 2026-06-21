using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Menu
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LabelEn { get; set; }

    public string? LabelAr { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
