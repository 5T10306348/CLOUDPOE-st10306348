using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class CartItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }  // Primary Key for SQL Server

        [Required]
        [MaxLength(100)]
        public string PartitionKey { get; set; }  // UserId or UserEmail

        [Required]
        [MaxLength(100)]
        public string RowKey { get; set; }  // Unique identifier for the Cart Item

        [Required]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [Required]
        public int ProductPrice { get; set; }

        [MaxLength(500)]
        public string ProductImageUri { get; set; }

        [MaxLength(100)]
        public string ProductRowKey { get; set; }  // Reference to Product's RowKey

        [MaxLength(100)]
        public string ProductPartitionKey { get; set; }  // Reference to Product's PartitionKey

        public int Quantity { get; set; } = 1;

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
