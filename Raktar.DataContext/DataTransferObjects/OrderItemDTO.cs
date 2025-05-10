namespace Raktar.DataContext.DataTransferObjects
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public int Quantity { get; set; }

        public int? FeedbackId { get; set; }
    }
    public class AddOrderItemDTO
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }

    }
}
