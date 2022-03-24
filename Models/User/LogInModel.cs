using System.ComponentModel.DataAnnotations;

namespace Models.User;

public class LogInModel
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}