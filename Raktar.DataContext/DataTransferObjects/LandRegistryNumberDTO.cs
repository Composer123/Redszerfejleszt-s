using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktar.DataContext.DataTransferObjects
{
    class LandRegistryNumberDTO : IAddressDTO
    {
        public int AddressId { get; set; }
        public SettlementDTO Settlement { get; set; }
        public string Contents { get; set; }
    }
    class LandRegistryNumberCreateDTO : IAddressCreateDTO
    {
        [Required]
        public int SettlementId { get; set; }
        [Required]
        public string Contents { get; set; }
    }
}
