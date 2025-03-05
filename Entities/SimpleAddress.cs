using System;
using System.Collections.Generic;

namespace Raktar;

public partial class SimpleAddress
{
    public int AddressId { get; set; }

    public int SettlementId { get; set; }

    public string? StreetName { get; set; }

    public string? StreetType { get; set; }

    public int? HouseNumber { get; set; }

    public int? StairwayNumber { get; set; }

    public int? FloorNumber { get; set; }

    public int? DoorNumber { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual Settlement Settlement { get; set; } = null!;
}
