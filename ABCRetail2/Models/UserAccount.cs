using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABCRetail2.Models
{
    public class UserAccount
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
        public string Username { get; set; }

        [Required]
        [MaxLength(200)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }  // Store hashed passwords for security

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
