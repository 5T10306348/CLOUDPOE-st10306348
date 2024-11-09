using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ABCRetail2.Models;
public class UserAccount
{
    [Key]
    public int Id { get; set; } // Primary Key as an int

    [Required]
    [MaxLength(100)]
    public string Username { get; set; }

    [Required]
    [MaxLength(200)]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }  // Store hashed passwords for security

    public DateTime Timestamp { get; set; } = DateTime.Now;
}
