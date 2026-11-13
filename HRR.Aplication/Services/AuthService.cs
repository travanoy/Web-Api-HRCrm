using HRR.Aplication.Interfaces;
using HRR.Aplication.Results;
using HRR.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace HRR.Aplication.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ILogger<AuthService> logger,
    IPasswordManager passwordManager,
    ITokenGenerator tokenGenerator) : IAuthService
{
    public async Task<Result> RegisterAsync(string username, string password)
    {
        if (await userRepository.ExistsByUsernameAsync(username))
        {
            logger.LogWarning("Registration rejected: username {Username} is taken", username);
            return Result.Failure($"Username '{username}' is already taken.", ErrorType.Conflict);
        }

        var user = new User
        {
            Username = username,
            PasswordHash = passwordManager.Hash(password),
            Role = UserRole.User
        };

        await userRepository.AddAsync(user);

        logger.LogInformation("User {Id} ({Username}) registered", user.Id, username);

        return Result.Success();
    }

    public async Task<Result<AuthResult>> LoginAsync(string username, string password)
    {
        var user = await userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            logger.LogWarning("Login failed: username {Username} not found", username);
            return Result<AuthResult>.Failure("Invalid username or password.", ErrorType.Unauthorized);
        }

        if (!passwordManager.Verify(password, user.PasswordHash))
        {
            logger.LogWarning("Login failed: wrong password for user {Id}", user.Id);
            return Result<AuthResult>.Failure("Invalid username or password.", ErrorType.Unauthorized);
        }

        await refreshTokenRepository.RemoveExpiredAsync(user.Id);

        var (accessToken, refreshToken) = await IssueTokensAsync(user);

        logger.LogInformation("User {Id} ({Username}) logged in", user.Id, username);
        return Result<AuthResult>.Success(new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken });
    }

    public async Task<Result<AuthResult>> RefreshAsync(string refreshToken)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken == null || storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            logger.LogWarning("Refresh rejected: token is invalid or expired");
            return Result<AuthResult>.Failure("Invalid or expired refresh token.", ErrorType.Unauthorized);
        }

        await refreshTokenRepository.RemoveExpiredAsync(storedToken.UserId);
        await refreshTokenRepository.RemoveAsync(storedToken);

        var (accessToken, newRefreshToken) = await IssueTokensAsync(storedToken.User);

        logger.LogInformation("Refresh token rotated for user {Id}", storedToken.UserId);
        return Result<AuthResult>.Success(new AuthResult { AccessToken = accessToken, RefreshToken = newRefreshToken });
    }

    private async Task<(string AccessToken, string RefreshToken)> IssueTokensAsync(User user)
    {
        var accessToken = tokenGenerator.GenerateAccessToken(user);
        var refreshToken = tokenGenerator.GenerateRefreshToken();

        await refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken.Token,
            UserId = user.Id,
            ExpiresAt = refreshToken.ExpiresAt
        });

        return (accessToken, refreshToken.Token);
    }
}
