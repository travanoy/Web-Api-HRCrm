using HRR.Aplication.Interfaces;
using HRR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HRR.Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task RemoveAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Remove(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task RemoveExpiredAsync(int userId)
    {
        var expired = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        if (expired.Count == 0)
            return;

        context.RefreshTokens.RemoveRange(expired);
        await context.SaveChangesAsync();
    }
}
