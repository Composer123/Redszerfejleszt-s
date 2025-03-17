using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class BlockDTO
    {
        public int StorageId { get; set; }

        public int Quantity { get; set; }

        public ProductDTO? Item { get; set; }
    }

    public class BlockAssignDTO
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public required ProductDTO Item { get; set; }
    }
}
