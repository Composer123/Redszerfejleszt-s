using System;
using System.Collections.Generic;

namespace Raktar.DataContext;

public partial class Address
{
    public int AddressId { get; set; }

    public virtual LandRegistryNumber? LandRegistryNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual SimpleAddress? SimpleAddress { get; set; }
}
