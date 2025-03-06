using Raktar.DataContext.Model;
using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
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
    public class SimpleAddressCreateDTO : IAddressDTO
    {
        [Required]
        public int AddressId { get; set; }
        [Required]
        public int SettlementId { get; set; }
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
