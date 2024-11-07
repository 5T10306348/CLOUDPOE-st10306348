using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class OrderMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key

        [Required]
        [MaxLength(200)]
        public string CustomerEmail { get; set; }

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }
    }
}
