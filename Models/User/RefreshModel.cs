using System.ComponentModel.DataAnnotations;

namespace Models.User;
public class RefreshModel
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}