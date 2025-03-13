using System.ComponentModel.DataAnnotations;

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
