using HRR.Aplication.Results;
using HRR.Domain.Entities;

namespace HRR.Aplication.Interfaces;

public interface ITokenGenerator
{
    string GenerateAccessToken(User user);
    RefreshTokenResult GenerateRefreshToken();
}
