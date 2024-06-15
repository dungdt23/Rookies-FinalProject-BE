using System.ComponentModel.DataAnnotations;

namespace AssetManagement.Application.Models;

public class LoginForm
{
    [Required]
    [MaxLength(200)]
    public string UserName { get; set; }

    [Required]
    [MaxLength(200)]
    public string Password { get; set; }
}
