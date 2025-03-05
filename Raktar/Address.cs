using System;
using System.Collections.Generic;

namespace Raktar;

public partial class Address
{
    public int AddressId { get; set; }

    public virtual LandRegistryNumber? LandRegistryNumber { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual SimpleAddress? SimpleAddress { get; set; }

    //This is a comment made by Gergo who is indeed a gamer (If you want some drawings, my commisions are open)
}
