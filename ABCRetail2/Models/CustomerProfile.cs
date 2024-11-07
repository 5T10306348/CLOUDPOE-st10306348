using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class CustomerProfile
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

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
