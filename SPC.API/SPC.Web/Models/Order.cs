using System;
using System.Collections.Generic;

namespace SPC.Web.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string PharmacyId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
        public string Status { get; set; }
    }
}
