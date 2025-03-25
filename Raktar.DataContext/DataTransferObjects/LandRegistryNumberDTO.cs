using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class LandRegistryNumberDTO : IAddressDTO
    {
        public int AddressId { get; set; }
        public SettlementDTO Settlement { get; set; }
        public string Contents { get; set; }
    }
    public class LandRegistryNumberCreateDTO : IAddressCreateDTO
    {
        [Required]
        public SettlementDTO Settlement { get; set; }
        [Required]
        public string Contents { get; set; }
    }
}
