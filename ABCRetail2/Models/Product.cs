using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class Product
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
        public string Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public int Price { get; set; }

        [MaxLength(500)]
        public string ImageUri { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
