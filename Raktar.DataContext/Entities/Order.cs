using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Order
{
    public int? UserId { get; set; }

    public int OrderId { get; set; }

    public int? DeliveryAdressId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Address? DeliveryAdress { get; set; }

    public virtual User? User { get; set; }
}
