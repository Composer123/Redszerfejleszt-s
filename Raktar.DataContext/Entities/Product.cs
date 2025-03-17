using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public string Type { get; set; }

    [Obsolete("Don't use stock. Use blocks service to get stock instead.")]
    public int Stock { get; set; } // TODO: Remove and create new migration. 

    public int MaxQuantityPerBlock { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
    public ICollection<Block> Blocks { get; set; }
}
