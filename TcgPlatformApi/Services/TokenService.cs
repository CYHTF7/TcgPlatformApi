using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;
using TcgPlatformApi.Settings;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly AppDbContext _context;

    public TokenService(IOptions<JwtSettings> jwtSettings, AppDbContext context)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateAccessToken(int playerId, string playerNickname)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, playerId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, playerNickname),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiresInHours),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }

    public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
    {
        var players = await _context.PlayerProfiles.ToListAsync();

        var player = players.FirstOrDefault(p =>
            BCrypt.Net.BCrypt.Verify(request.RefreshToken, p.RefreshTokenHash) &&
            p.RefreshTokenExpiryTime > DateTime.UtcNow);

        if (player == null)
        {
            throw new AppException(
                userMessage: "Invalid or expired refresh token",
                statusCode: HttpStatusCode.Unauthorized,
                logMessage: $"[TokenService] Invalid refresh token attempt"
            );
        }

        var newAccessToken = GenerateAccessToken(player.Id, player.Nickname);

        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(newRefreshToken);

        player.RefreshTokenHash = newRefreshTokenHash;
        player.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays);

        await _context.SaveChangesAsync();

        return (new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }
}

