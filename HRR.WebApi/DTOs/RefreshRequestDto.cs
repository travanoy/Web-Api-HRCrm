using System.ComponentModel.DataAnnotations;

namespace HRR.WebApi.DTOs;

public class RefreshRequestDto
{
    [Required(ErrorMessage = "Refresh token is required.")]
    public string RefreshToken { get; set; } = string.Empty;
}
