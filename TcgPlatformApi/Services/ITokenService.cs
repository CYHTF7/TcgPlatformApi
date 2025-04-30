using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(int playerId, string playerNickname);
        string GenerateRefreshToken();
        Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request);
    }
}
