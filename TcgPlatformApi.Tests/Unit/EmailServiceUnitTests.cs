using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Net.Mail;
using TcgPlatformApi.Controllers;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Services;
using TcgPlatformApi.Smtp;

namespace TcgPlatformApi.Tests.Unit
{
    public class EmailServiceUnitTests
    {
        [Fact]
        public async Task EmailService_ValidRequest_SendsEmailAndReturnsTrue() 
        {
            var mockSmtpClient = new Mock<ISmtpClient>();

            mockSmtpClient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>()))
                .Returns(Task.CompletedTask);

            var emailService = new EmailService(mockSmtpClient.Object);

            var result = await emailService.SendEmailAsync("test@gmail.com", "123456", 1);

            Assert.True(result);

            mockSmtpClient.Verify(x => x.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_InvalidRequest_ThrowsAppException()
        {
            var mockSmtpClient = new Mock<ISmtpClient>();

            var emailService = new EmailService(mockSmtpClient.Object);

            var ext1 = await Assert.ThrowsAsync<AppException>(() => emailService.SendEmailAsync("", "123456", 1));

            Assert.Equal(HttpStatusCode.BadRequest, ext1.StatusCode);
            Assert.Equal("Email was not sent", ext1.UserMessage);

            var ext2 = await Assert.ThrowsAsync<AppException>(() => emailService.SendEmailAsync("test@gmail.com", "", 2));

            Assert.Equal(HttpStatusCode.BadRequest, ext1.StatusCode);
            Assert.Equal("Email was not sent", ext1.UserMessage);
        }
    }
}
