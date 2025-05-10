using Raktar.DataContext.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raktar.DataContext.DataTransferObjects
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int DeliveryAdressId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItemDTO> OrderItems { get; set; }
    }

    public class OrderCreateDTO
    {
        public int UserId { get; set; }
        public int DeliveryAdressId { get; set; }
    }

    public interface IOrderStatusDTO
    {
        public OrderStatus OrderStatus { get; }
    }

    public class OrderCancelStatusDTO : IOrderStatusDTO
    {
        public OrderStatus OrderStatus { get; } = OrderStatus.Cancelled;
    }

    public class OrderStatusDelliveryAcceptDTO : IOrderStatusDTO
    {
        public OrderStatus OrderStatus { get; } = OrderStatus.Accepted;
        public DateTime DelliveryDate { get; set; }
    }
    public class OrderStatusDTO : IOrderStatusDTO
    {
        public OrderStatus OrderStatus { get; init; }
    }
}
