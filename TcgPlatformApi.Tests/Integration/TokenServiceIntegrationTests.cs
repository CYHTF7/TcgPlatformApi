using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;
using TcgPlatformApi.Settings;

namespace TcgPlatformApi.Tests
{
    public class TokenServiceIntegrationTests
    {
        [Fact]
        public async Task RefreshToken_ValidRequest_ReturnsNewTokensAndUpdatesDatabase()
        {
            var oldRefreshToken = "oldrefreshtoken";

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
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new AppDbContext(dbContextOptions))
            {
                var testPlayer = new PlayerProfile
                {
                    Id = 1,
                    Nickname = "qwerty",
                    Email = "qwerty123@gmail.com",
                    PasswordHash = "123123123",
                    RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(oldRefreshToken),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
                };

                context.PlayerProfiles.Add(testPlayer);
                await context.SaveChangesAsync();
            }

            using (var dbContext = new AppDbContext(dbContextOptions))
            {
                var tokenService = new TokenService(mockOptions.Object, dbContext);

                var request = new RefreshTokenRequest
                {
                    PlayerId = 1,
                    RefreshToken = oldRefreshToken
                };

                var response = await tokenService.RefreshToken(request);

                Assert.NotNull(response);
                Assert.NotNull(response.AccessToken);
                Assert.NotNull(response.RefreshToken);
                Assert.NotEqual(oldRefreshToken, response.RefreshToken);

                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedAccessToken = tokenHandler.ReadJwtToken(response.AccessToken);

                Assert.Equal("1", decodedAccessToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
                Assert.Equal("qwerty", decodedAccessToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);

                var updatedPlayer = await dbContext.PlayerProfiles.FirstAsync(u => u.Id == 1);
                Assert.True(BCrypt.Net.BCrypt.Verify(response.RefreshToken, updatedPlayer.RefreshTokenHash));
                Assert.True(updatedPlayer.RefreshTokenExpiryTime > DateTime.UtcNow);
            }
        }

        [Fact]
        public async Task RefreshToken_ExpiredToken_ReturnsAppException()
        {
            var expiredRefreshToken = "expiredtoken";

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
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new AppDbContext(dbContextOptions))
            {
                var testPlayer = new PlayerProfile
                {
                    Id = 2,
                    Nickname = "qwerty",
                    Email = "qwerty123@gmail.com",
                    PasswordHash = "123123123",
                    RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(expiredRefreshToken),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
                };

                context.PlayerProfiles.Add(testPlayer);
                await context.SaveChangesAsync();
            }

            using (var dbContext = new AppDbContext(dbContextOptions))
            {
                var tokenService = new TokenService(mockOptions.Object, dbContext);

                var request = new RefreshTokenRequest
                {
                    PlayerId = 2,
                    RefreshToken = expiredRefreshToken
                };

                var response = await Assert.ThrowsAsync<AppException>(() => tokenService.RefreshToken(request));

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
                Assert.Equal("Invalid or expired refresh token", response.UserMessage);
            }
        }

        [Fact]
        public async Task RefreshToken_UnauthorizedProfile_ReturnsAppException() 
        {
            var refreshToken = "refreshtoken";

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
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var dbContext = new AppDbContext(dbContextOptions))
            {
                var tokenService = new TokenService(mockOptions.Object, dbContext);

                var request = new RefreshTokenRequest
                {
                    PlayerId = 0123,
                    RefreshToken = refreshToken
                };

                var response = await Assert.ThrowsAsync<AppException>(() => tokenService.RefreshToken(request));

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
                Assert.Equal("Unauthorized", response.UserMessage);
            }
        }

        [Fact]
        public async Task RefreshToken_TokenCloseToExpiry_ReturnsNewToken() 
        {
            var oneMinRefreshToken = "oneminrefreshtoken";

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
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            using (var context = new AppDbContext(dbContextOptions))
            {
                var testPlayer = new PlayerProfile
                {
                    Id = 3,
                    Nickname = "qwerty",
                    Email = "qwerty123@gmail.com",
                    PasswordHash = "123123123",
                    RefreshTokenHash = BCrypt.Net.BCrypt.HashPassword(oneMinRefreshToken),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(1)
                };

                context.PlayerProfiles.Add(testPlayer);
                await context.SaveChangesAsync();
            }

            using (var dbContext = new AppDbContext(dbContextOptions))
            {
                var tokenService = new TokenService(mockOptions.Object, dbContext);

                var request = new RefreshTokenRequest
                {
                    PlayerId = 3,
                    RefreshToken = oneMinRefreshToken
                };

                var response = await tokenService.RefreshToken(request);

                Assert.NotNull(response);
                Assert.NotNull(response.AccessToken);
                Assert.NotNull(response.RefreshToken);
                Assert.NotEqual(oneMinRefreshToken, response.RefreshToken);

                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedAccessToken = tokenHandler.ReadJwtToken(response.AccessToken);

                Assert.Equal("3", decodedAccessToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
                Assert.Equal("qwerty", decodedAccessToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value);

                var updatedPlayer = await dbContext.PlayerProfiles.FirstAsync(u => u.Id == 3);
                Assert.True(BCrypt.Net.BCrypt.Verify(response.RefreshToken, updatedPlayer.RefreshTokenHash));
                Assert.True(updatedPlayer.RefreshTokenExpiryTime > DateTime.UtcNow);
            }
        }
    }
}


