using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key

        [Required]
        [MaxLength(100)]
        public string PartitionKey { get; set; }

        [Required]
        [MaxLength(100)]
        public string RowKey { get; set; }

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; }

        [Required]
        [MaxLength(200)]
        public string CustomerEmail { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(20)]
        public string ZipCode { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string Province { get; set; }

        [MaxLength(200)]
        public string ProductName { get; set; }

        public int ProductPrice { get; set; }

        [MaxLength(500)]
        public string ProductImageUri { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
