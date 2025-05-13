using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using TcgPlatformApi.Data;
using TcgPlatformApi.Settings;

namespace TcgPlatformApi.Tests
{
    public class TokenServiceUnitTests
    {
        [Fact]
        public void GenerateAccessToken_ReturnsValidToken()
        {
            var jwtSettings = new JwtSettings
            {
                SecretKey = "12345678901234567890123456789012",
                RefreshTokenSecret = "12123456789012345678901234567890",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiresInHours = 1,
                RefreshTokenExpiresInDays = 7
            };

            var mockOptions = new Mock<IOptions<JwtSettings>>();
            mockOptions.Setup(x => x.Value).Returns(jwtSettings);

            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var dbContext = new AppDbContext(dbContextOptions);

            var tokenService = new TokenService(mockOptions.Object, dbContext);
            int userId = 123;
            string userName = "qwerty";

            var response = tokenService.GenerateAccessToken(userId, userName);

            Assert.NotNull(response);
            Assert.NotEmpty(response);

            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(response);

            var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            var userNameClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value;

            Assert.Equal(userId.ToString(), userIdClaim);
            Assert.Equal(userName, userNameClaim);
        }
    }
}
