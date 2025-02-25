namespace SPC.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int DrugId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice; 
    }
}
