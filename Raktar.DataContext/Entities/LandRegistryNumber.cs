using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class LandRegistryNumber
{
    public int AddressId { get; set; }

    public int SettlementId { get; set; }

    public string Contents { get; set; }

    public Address Address { get; set; } = null!;

    public Settlement Settlement { get; set; } = null!;
}
