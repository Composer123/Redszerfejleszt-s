using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktar.DataContext.DataTransferObjects
{
    public class SettlementDTO
    {
        public int SettlementId { get; set; }
        [Range(1000, 9999)]
        public int PostCode { get; set; }
        public required string SettlementName { get; set; }
    }
}
