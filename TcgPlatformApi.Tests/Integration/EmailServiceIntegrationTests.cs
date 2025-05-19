using System.Net;
using System.Text;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Moq;
using TcgPlatformApi.Smtp;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;


namespace TcgPlatformApi.Tests.Integration
{
    public class EmailServiceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        private readonly Mock<ISmtpClient> _smtpClientMock = new();

        public EmailServiceIntegrationTests()
        {
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("OffJwt");

                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<ISmtpClient>();
                        services.AddSingleton(_smtpClientMock.Object);
                    });
                });

            _client = factory.CreateClient();
        }

        [Fact]
        public async Task SendEmailAsync_InvalidDomain_ThrowsBadRequestObjectResult()
        {
            var content = new StringContent(JsonConvert.SerializeObject(new
            {
                ToEmail = "teat@test.test",
                Code = "test",
                Variant = 1
            }), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("api/email/email", content);

            var result = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Server Error: {errorResponse}");
            }

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Email domain is not allowed", result);
        }
    }
}



