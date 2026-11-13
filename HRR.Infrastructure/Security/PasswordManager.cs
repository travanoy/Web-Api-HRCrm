using HRR.Aplication.Interfaces;
using HRR.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HRR.Infrastructure.Security;

public class PasswordManager : IPasswordManager
{
    private readonly PasswordHasher<User> hasher = new();

    public string Hash(string password)
    {
        return hasher.HashPassword(null!, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var result = hasher.VerifyHashedPassword(null!, passwordHash, password);
        return result != PasswordVerificationResult.Failed;
    }
}
