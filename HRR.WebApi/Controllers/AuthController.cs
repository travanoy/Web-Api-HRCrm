using HRR.Aplication.Services;
using HRR.WebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRR.WebApi.Controllers;

[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto request)
    {
        var result = await authService.RegisterAsync(request.Username, request.Password);

        if (!result.IsSuccess)
            return ToErrorResponse(result);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request)
    {
        var result = await authService.LoginAsync(request.Username, request.Password);

        if (!result.IsSuccess)
            return ToErrorResponse(result);

        return Ok(new AuthResponseDto
        {
            AccessToken = result.Value!.AccessToken,
            RefreshToken = result.Value!.RefreshToken
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshRequestDto request)
    {
        var result = await authService.RefreshAsync(request.RefreshToken);

        if (!result.IsSuccess)
            return ToErrorResponse(result);

        return Ok(new AuthResponseDto
        {
            AccessToken = result.Value!.AccessToken,
            RefreshToken = result.Value!.RefreshToken
        });
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult<MeResponseDto> Me()
    {
        var response = new MeResponseDto
        {
            Id = int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : 0,
            Username = User.FindFirst("name")?.Value ?? string.Empty,
            Role = User.FindFirst("role")?.Value ?? string.Empty
        };

        return Ok(response);
    }
}
