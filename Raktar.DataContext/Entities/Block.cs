using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Block
{
    public int? ProductId { get; set; }

    public int StorageId { get; set; }

    public int Quantity { get; set; }

    public Product? Product { get; set; }
}
