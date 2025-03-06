using System;
using System.Collections.Generic;

namespace Raktar.DataContext;

public partial class Settlement
{
    public int SettlementId { get; set; }

    public int? PostCode { get; set; }

    public string? SettlementName { get; set; }

    public virtual ICollection<LandRegistryNumber> LandRegistryNumbers { get; set; } = new List<LandRegistryNumber>();

    public virtual ICollection<SimpleAddress> SimpleAddresses { get; set; } = new List<SimpleAddress>();
}
