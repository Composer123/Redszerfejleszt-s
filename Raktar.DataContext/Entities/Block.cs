using System;
using System.Collections.Generic;

namespace Raktar.DataContext;

public partial class Block
{
    public int? ProductId { get; set; }

    public int? StorageId { get; set; }

    public int? Quantity { get; set; }

    public virtual Product? Product { get; set; }
}
