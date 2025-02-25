using System.ComponentModel.DataAnnotations;

namespace SPC.API.Models
{
    public class SupplierDrugOrder
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public int DrugId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
