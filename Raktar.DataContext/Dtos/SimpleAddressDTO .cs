using Raktar.DataContext.Entities;
using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.Dtos
{
    public class SimpleAddressDTO : IAddressDTO
    {
        public int AddressId { get; set; }
        public SettlementDTO Settlement { get; set; }
        public string StreetName { get; set; }
        public PostalStreetType StreetType { get; set; }

        public int HouseNumber { get; set; }

        public int? StairwayNumber { get; set; }

        public int? FloorNumber { get; set; }

        public int? DoorNumber { get; set; }
    }
    public class SimpleAddressCreateDTO : IAddressCreateDTO
    {
        [Required]
        public SettlementDTO Settlement { get; set; }
        [Required]
        public string StreetName { get; set; }
        [Required]
        public PostalStreetType StreetType { get; set; }
        [Required]
        public int HouseNumber { get; set; }

        public int? StairwayNumber { get; set; }

        public int? FloorNumber { get; set; }

        public int? DoorNumber { get; set; }
    }
}
