namespace SPC.API.Models
{
    public class SupplierDrug
    {
        public int Id { get; set; }
        public int SupplierId { get; set; } // Foreign key reference remains
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockLevel { get; set; }
        public DateTime ManufactureDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
