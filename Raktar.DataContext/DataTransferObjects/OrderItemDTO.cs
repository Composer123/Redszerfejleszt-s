using System.ComponentModel.DataAnnotations;

namespace Raktar.DataContext.DataTransferObjects
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public int? FeedbackId { get; set; }
        public string ProductName { get; set; }
    }
    public class AddOrderItemDTO
    {
        public int ProductId { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

    }
}
