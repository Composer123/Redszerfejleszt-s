using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Address
{
    public int AddressId { get; set; }

    public LandRegistryNumber? LandRegistryNumber { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public SimpleAddress? SimpleAddress { get; set; }
}
