using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;

public partial class Settlement
{
    public int SettlementId { get; set; }

    public int PostCode { get; set; }

    public string SettlementName { get; set; }

    public ICollection<LandRegistryNumber> LandRegistryNumbers { get; set; } = new List<LandRegistryNumber>();

    public ICollection<SimpleAddress> SimpleAddresses { get; set; } = new List<SimpleAddress>();
}
