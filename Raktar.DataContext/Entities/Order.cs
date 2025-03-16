using System;
using System.Collections.Generic;

namespace Raktar.DataContext.Entities;
public enum OrderStatus
{
    Pending,
    Accepted,
    ReadyForDelivery,
    Delivered,
    Cancelled,
    Undeliverible
}
public partial class Order
{

    public int UserId { get; set; }

    public int OrderId { get; set; }

    public int DeliveryAdressId { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public OrderStatus Status { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }

    public Address DeliveryAdress { get; set; }

    public User User { get; set; }

}
