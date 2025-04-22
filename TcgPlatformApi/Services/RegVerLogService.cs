using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Models;
using System.Security.Authentication;
using System.Security;

namespace TcgPlatformApi.Services
{
    public class RegVerLogService : IRegVerLogService
    {
        private readonly AppDbContext _context;

        public RegVerLogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfileRegDTO> Register(RegRequest request)
        {
            Console.WriteLine($"Incoming request: Nickname={request.Nickname}, Email={request.Email}, Password={request.Password}");

            if (await _context.PlayerProfiles.AnyAsync(p => p.Email == request.Email))
            {
                throw new ArgumentException("Email already in use.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //random code (simple variant)
            string verificationCode = Guid.NewGuid().ToString().Substring(0, 6);

            var newProfile = new PlayerProfile
            {
                Nickname = request.Nickname,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordResetCode = "NONE",
                IsVerified = false,
                EmailCode = verificationCode
            };

            _context.PlayerProfiles.Add(newProfile);
            await _context.SaveChangesAsync();

            await SendEmailAsync(request.Email, verificationCode, 1);

            return new PlayerProfileRegDTO
            {
                Id = newProfile.Id,
                Nickname = newProfile.Nickname,
                Email = newProfile.Email
            };
        }

        public async Task<bool> SendEmailAsync(string toEmail, string code, int variant)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(code))
            {
                return false;
            }

            try
            {
                //setup
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("wowtcgonline@gmail.com", "uoce qpok olkn rgzj"),
                    EnableSsl = true,
                };

                if (variant == 1)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("wowtcgonline@gmail.com", "WOWTCGONLINE"),
                        Subject = "WOWTCG - Verification Code",
                        Body = $"Your Verification Code: {code}",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);

                    return true;
                }

                if (variant == 2)
                {
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("wowtcgonline@gmail.com", "WOWTCGONLINE"),
                        Subject = "WOWTCG - Reset Password Code",
                        Body = $"Your Reset Password Code: {code}",
                        IsBodyHtml = false,
                    };
                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<bool> VerifyAccount(VerRequest request)
        {
            var user = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                throw new ArgumentException("Account not found!");
            }

            if (user.IsVerified)
            {
                throw new InvalidOperationException("Account is already verified!");
            }

            if (user.EmailCode != request.EmailCode)
            {
                throw new ArgumentException("Invalid verification code!");
            }

            user.IsVerified = true;
            user.EmailCode = "DONE";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PlayerProfileLogDTO> Login(LogRequest request)
        {
            var user = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                throw new ArgumentException("Invalid email or password");
            }

            if (!user.IsVerified)
            {
                throw new VerificationException("Account not verified. Please check your email");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new AuthenticationException("Invalid email or password");
            }

            var playerProfileLogDTO = new PlayerProfileLogDTO
            {
                Id = user.Id,
                Nickname = user.Nickname,
                Level = user.Level,
                Gold = user.Gold,
                Experience = user.Experience,
                AvatarPath = user.AvatarPath,
                BattlesPlayed = user.BattlesPlayed
            };

            return playerProfileLogDTO;
        }

        public async Task<bool> ResetPassword(RessPassRequest request)
        {
            var user = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                throw new ArgumentException("Invalid data to reset password!");
            }

            if (!user.IsVerified)
            {
                throw new VerificationException("Account is not verified!");
            }

            string resetCode = Guid.NewGuid().ToString().Substring(0, 6);

            user.PasswordResetCode = resetCode;
            await _context.SaveChangesAsync();

            var emailResult = await SendEmailAsync(request.Email, resetCode, 2);

            if (!emailResult)
            {
                throw new Exception("Failed to send email");
            }

            return true;
        }

        public async Task<bool> VerifyResetPassword(VerRessPassRequest request)
        {
            var user = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                throw new ArgumentNullException("Invalid data to verify reset password!");
            }

            if (user.PasswordResetCode != request.PasswordResetCode)
            {
                throw new ArgumentNullException("Invalid verification code!");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordHash = passwordHash;
            user.PasswordResetCode = "CHANGED";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
