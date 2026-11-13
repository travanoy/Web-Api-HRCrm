using HRR.Domain.Entities;

namespace HRR.Aplication.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task AddAsync(RefreshToken refreshToken);
    Task RemoveAsync(RefreshToken refreshToken);
    Task RemoveExpiredAsync(int userId);
}
