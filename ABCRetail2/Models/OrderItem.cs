using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }  // Price per unit
    }
}
