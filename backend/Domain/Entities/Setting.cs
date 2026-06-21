using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Setting
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string? Value { get; set; }

    public string Type { get; set; } = null!;

    public string? Group { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
