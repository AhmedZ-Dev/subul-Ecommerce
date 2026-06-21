using System;
using System.Collections.Generic;

namespace backend.Domain.Entities;

public partial class CollectionProduct
{
    public long Id { get; set; }

    public long CollectionId { get; set; }

    public long ProductId { get; set; }

    public int SortOrder { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Collection Collection { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
