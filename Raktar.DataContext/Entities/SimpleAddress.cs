using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.Entities;

public partial class SimpleAddress
{
    public int AddressId { get; set; }

    public int SettlementId { get; set; }

    public string StreetName { get; set; }

    public PostalStreetType StreetType { get; set; }

    public int HouseNumber { get; set; }

    public int? StairwayNumber { get; set; }

    public int? FloorNumber { get; set; }

    public int? DoorNumber { get; set; }

    public Address Address { get; set; } = null!;

    public Settlement Settlement { get; set; } = null!;
}

public enum PostalStreetType
{
    /// <summary>Akna</summary>
    [Display(Name = "Akna")]
    Pit,
    /// <summary>Allé</summary>
    [Display(Name = "Allé")]
    Alley,
    /// <summary>Alsó</summary>
    [Display(Name = "Alsó")]
    Lower,
    /// <summary>Alsósor</summary>
    [Display(Name = "Alsósor")]
    LowerRow,
    /// <summary>Árok</summary>
    [Display(Name = "Árok")]
    Trench,
    /// <summary>Átjáró</summary>
    [Display(Name = "Átjáró")]
    Passageway,
    /// <summary>Bekötőút</summary>
    [Display(Name = "Bekötőút")]
    AccesRoad,
    /// <summary>Bokor</summary>
    [Display(Name = "Bokor")]
    Bush,
    /// <summary>Dűlő</summary>
    [Display(Name = "Dűlő")]
    Vineyard,
    /// <summary>Dűlőút</summary>
    [Display(Name = "Dűlőút")]
    VineyardStreet,
    /// <summary>Erdősor</summary>
    [Display(Name = "Erdősor")]
    ForrestLine,
    /// <summary>Fasor</summary>
    [Display(Name = "Fasor")]
    AvenueTreeRow,
    /// <summary>Felső rakpart</summary>
    [Display(Name = "Felső rakpart")]
    UpperQuay,
    /// <summary>Felsősor</summary>
    [Display(Name = "Felsősor")]
    UpperRow,
    /// <summary>Forduló</summary>
    [Display(Name = "Forduló")]
    Turn,
    /// <summary>Főtér</summary>
    [Display(Name = "Főtér")]
    MainSquare,
    /// <summary>Főút</summary>
    [Display(Name = "Főút")]
    MainRoad,
    /// <summary>Gát</summary>
    [Display(Name = "Gát")]
    Dam,
    /// <summary>Határ</summary>
    [Display(Name = "Határ")]
    Frontier,
    /// <summary>Határsor</summary>
    [Display(Name = "Határsor")]
    FrontierRow,
    /// <summary>Határút</summary>
    [Display(Name = "Határút")]
    FrontierRoad,
    /// <summary>Ipartelep</summary>
    [Display(Name = "Ipartelep")]
    IndustrialEstate,
    /// <summary>Kapu</summary>
    [Display(Name = "Kapu")]
    Gate,
    /// <summary>Kert</summary>
    [Display(Name = "Kert")]
    Garden,
    /// <summary>Kertsor</summary>
    [Display(Name = "Kertsor")]
    GardenRow,
    /// <summary>Korzó</summary>
    [Display(Name = "Korzó")]
    Promenade,
    /// <summary>Környék</summary>
    [Display(Name = "Környék")]
    Neighborhood,
    /// <summary>Körönd</summary>
    [Display(Name = "Körönd")]
    Circus,
    /// <summary>Körtér</summary>
    [Display(Name = "Körtér")]
    CircusFormal,
    /// <summary>Körút</summary>
    [Display(Name = "Körút")]
    Boulevard,
    /// <summary>Köz</summary>
    [Display(Name = "Köz")]
    Gap,
    /// <summary>Lakópark</summary>
    [Display(Name = "Lakópark")]
    ReditentialPark,
    /// <summary>Lakótelep</summary>
    [Display(Name = "Lakótelep")]
    HousingEstate,
    /// <summary>Lejáró</summary>
    [Display(Name = "Lejáró")]
    Hatch,
    /// <summary>Lejtő</summary>
    [Display(Name = "Lejtő")]
    Slope,
    /// <summary>Lépcső</summary>
    [Display(Name = "Lépcső")]
    Stairscase,
    /// <summary>Lépcsősor</summary>
    [Display(Name = "Lépcsősor")]
    FlightOfStairs,
    /// <summary>Liget</summary>
    [Display(Name = "Liget")]
    Grove,
    /// <summary>Major</summary>
    [Display(Name = "Major")]
    Croft,
    /// <summary>Majorság</summary>
    [Display(Name = "Majorság")]
    Manor,
    /// <summary>Mélyút</summary>
    [Display(Name = "Mélyút")]
    SteepRoad,
    /// <summary>Negyed</summary>
    [Display(Name = "Negyed")]
    Square,
    /// <summary>Oldal</summary>
    [Display(Name = "Oldal")]
    Side,
    /// <summary>Orom</summary>
    [Display(Name = "Orom")]
    Peak,
    /// <summary>Országút</summary>
    [Display(Name = "Országút")]
    Highway,
    /// <summary>Ösvény</summary>
    [Display(Name = "Ösvény")]
    Path,
    /// <summary>Park</summary>
    [Display(Name = "Park")]
    Park,
    /// <summary>Part</summary>
    [Display(Name = "Part")]
    Beach,
    /// <summary>Pincesor</summary>
    [Display(Name = "Pincesor")]
    CellarRow,
    /// <summary>Puszta</summary>
    [Display(Name = "Puszta")]
    Waste,
    /// <summary>Sétány</summary>
    [Display(Name = "Sétány")]
    PromenadeFormal,
    /// <summary>Sikátor</summary>
    [Display(Name = "Sikátor")]
    Alleyway,
    /// <summary>Sor</summary>
    [Display(Name = "Sor")]
    Row,
    /// <summary>Sugárút</summary>
    [Display(Name = "Sugárút")]
    Avenue,
    /// <summary>Szállás</summary>
    [Display(Name = "Szállás")]
    Quarters,
    /// <summary>Szektor</summary>
    [Display(Name = "Szektor")]
    Sector,
    /// <summary>Szél</summary>
    [Display(Name = "Szél")]
    Edge,
    /// <summary>Szer</summary>
    [Display(Name = "Szer")]
    Haunt,
    /// <summary>Sziget</summary>
    [Display(Name = "Sziget")]
    Island,
    /// <summary>Szőlőhegy</summary>
    [Display(Name = "Szőlőhegy")]
    VineyardGrapeHill,
    /// <summary>Tag</summary>
    [Display(Name = "Tag")]
    PlotOfLand,
    /// <summary>Tanya</summary>
    [Display(Name = "Tanya")]
    Ranch,
    /// <summary>Telep</summary>
    [Display(Name = "Telep")]
    Yard,
    /// <summary>Tér</summary>
    [Display(Name = "Tér")]
    SquarePlace,
    /// <summary>Tere</summary>
    [Display(Name = "Tere")]
    SquareOfSomething,
    /// <summary>Tető</summary>
    [Display(Name = "Tető")]
    Top,
    /// <summary>Udvar</summary>
    [Display(Name = "Udvar")]
    Court,
    /// <summary>Utca</summary>
    [Display(Name = "Utca")]
    Street,
    /// <summary>Út</summary>
    [Display(Name = "Út")]
    Road,
    /// <summary>Útja</summary>
    [Display(Name = "Útja")]
    RoadOfSomething,
    /// <summary>Üdülőpart</summary>
    [Display(Name = "Üdülőpart")]
    BeachResort,
    /// <summary>Üdülősor</summary>
    [Display(Name = "Üdülősor")]
    ResortRow,
    /// <summary>Üdülőtelep</summary>
    [Display(Name = "Üdülőtelep")]
    ResortEstate,
    /// <summary>Vár</summary>
    [Display(Name = "Vár")]
    Castle,
    /// <summary>Várkert</summary>
    [Display(Name = "Várkert")]
    CastleCourt,
    /// <summary>Város</summary>
    [Display(Name = "Város")]
    Town,
    /// <summary>Villasor</summary>
    [Display(Name = "Villasor")]
    VillaRow,
    /// <summary>Völgy</summary>
    [Display(Name = "Völgy")]
    Valley,
    /// <summary>Zug</summary>
    [Display(Name = "Zug")]
    Nook
}
