using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace ABCRetail2.Models;
public class UserAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } // Primary key with Identity

    [Required]
    [MaxLength(200)]
    public string Username { get; set; }

    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}
