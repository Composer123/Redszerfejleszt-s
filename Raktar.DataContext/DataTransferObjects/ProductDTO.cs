using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public string Type { get; set; }

        //public int Stock { get; set; }

        public int MaxQuantityPerBlock { get; set; }
    }
    /// <summary>
    /// To be used to assign product to a certain block.
    /// </summary>
    public class ProductAssignDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int MaxQuantityPerBlock { get; set; }
    }
}
