using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class CustomerProfile
    {
        [Key]
        public int Id { get; set; } // Primary Key, auto-incremented

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }

        [Required]
        [MaxLength(255)]
        public string CustomerEmail { get; set; }
    }
}