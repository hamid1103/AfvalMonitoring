using System.ComponentModel.DataAnnotations;

namespace AfvalMonitoring.Models;

public class AppUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;
}
