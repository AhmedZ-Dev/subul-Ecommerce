using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class Wishlist
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long ProductId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
