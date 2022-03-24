using System.ComponentModel.DataAnnotations;

namespace Models.User;

public class ForgotPasswordModel
{
    [Required]
    public string UsernameOrEmail { get; set; } = string.Empty;
}