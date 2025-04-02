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
    }

    public class OrderCreateDTO
    {
        public int UserId { get; set; }
        public int DeliveryAdressId { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}
