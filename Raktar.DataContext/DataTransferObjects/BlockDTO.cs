using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class BlockDTO
    {
        public int BlockId { get; set; }

        public int Quantity { get; set; }

        public ProductDTO? Item { get; set; }
    }

    public class BlockAssignOrRemoveDTO
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public int ProductId { get; set; }
    }
}
