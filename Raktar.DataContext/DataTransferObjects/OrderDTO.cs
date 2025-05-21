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
        public int? CarrierId { get; set; }
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
        public ICollection<AddOrderItemDTO> OrderItems { get; set; } = [];
    }

    public class OrderStatusDTO
    {
        public OrderStatus OrderStatus { get; set; }
        public DateTime? DelliveryDate { get; set; }
    }

    public class ChangeOrderDTO
    {
        /// <summary>
        /// The new order items for the order.
        /// </summary>
        public ICollection<AddOrderItemDTO> NewItems { get; set; } = new List<AddOrderItemDTO>();

        /// <summary>
        /// (Optional) Updated address for the order.
        /// If provided, the order’s DeliveryAdress will be replaced.
        /// </summary>
        public SimpleAddressDTO? UpdatedAddress { get; set; }
    }

    public class ChangeOrderAdminDTO
    {
        /// <summary>
        /// The new order items for the order.
        /// </summary>
        public ICollection<AddOrderItemDTO> NewItems { get; set; } = new List<AddOrderItemDTO>();

        /// <summary>
        /// (Optional) Updated address for the order.
        /// If provided, the order’s DeliveryAdress will be replaced.
        /// </summary>
        public SimpleAddressDTO? UpdatedAddress { get; set; }
        public int? CarrierId { get; set; }
    }

    public class ChangeDeliveryDateDTO
    {
        public DateTime NewDeliveryDate { get; set; }
    }

    public class AssignOrderCarrierDTO
    {
        public int CarrierId { get; set; }
    }

}
