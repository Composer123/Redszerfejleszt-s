using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class LandRegistryNumberDTO : IAddressDTO
    {
        public int AddressId { get; set; }
        public string StreetName { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public SettlementDTO Settlement { get; set; }
        public string Contents { get; set; }
    }
    class LandRegistryNumberCreateDTO : IAddressCreateDTO
    {
        [Required]
        public SettlementDTO Settlement { get; set; }
        [Required]
        public string Contents { get; set; }
    }
}
