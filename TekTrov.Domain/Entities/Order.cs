//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TekTrov.Domain.Enums;

//namespace TekTrov.Domain.Entities
//{
//    public class Order : BaseEntity
//    {
//        public int Id { get; set; }

//        public int UserId { get; set; }
//        public User? User { get; set; }

//        public DateTime OrderDate { get; set; }
//        public decimal TotalAmount { get; set; }

//        public OrderStatus Status { get; set; } = OrderStatus.Pending;

//        public ICollection<OrderItem> OrderItems { get; set; }
//            = new List<OrderItem>();
//    }
//}
