using System.ComponentModel.DataAnnotations;

namespace HRR.WebApi.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(3, ErrorMessage = "Username must be at least 3 characters long.")]
    [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(3, ErrorMessage = "Password must be at least 3 characters long.")]
    public string Password { get; set; } = string.Empty;
}
