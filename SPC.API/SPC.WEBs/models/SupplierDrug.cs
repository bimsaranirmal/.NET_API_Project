using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SPC.Web.Models;

namespace SPC.Web.Models
{
    public class SupplierDrug
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockLevel { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}