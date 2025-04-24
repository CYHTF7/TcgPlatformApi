using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Models;
using System.Security.Authentication;
using System.Security;
using TcgPlatformApi.Exceptions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

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
            if (await _context.PlayerProfiles.AnyAsync(p => p.Email == request.Email))
            {
                throw new AppException(
                    userMessage: "Email already in use",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[RegVerLogService] Email already in use: {request.Email}"
                );
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            //random code (simple variant)
            string verificationCode = Guid.NewGuid().ToString().Substring(0, 8);

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

            return true;
        }

        public async Task<bool> VerifyAccount(VerRequest request)
        {
            var player = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (player == null)
            {
                throw new AppException(
                    userMessage: "Player profile not found",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[RegVerLogService] Player profile not found: {player}"
                );
            }

            if (player.IsVerified)
            {
                throw new AppException(
                    userMessage: "Account is already verified",
                    statusCode: HttpStatusCode.Conflict,
                    logMessage: $"[RegVerLogService] Account is already verified: {player}"
                );
            }

            if (player.EmailCode != request.EmailCode)
            {
                throw new AppException(
                    userMessage: "Invalid verification code",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[RegVerLogService] Invalid verification code: {request.EmailCode}"
                );
            }

            player.IsVerified = true;
            player.EmailCode = "DONE";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PlayerProfileLogDTO> Login(LogRequest request)
        {
            var player = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (player == null)
            {
                throw new AppException(
                    userMessage: "Invalid email",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[RegVerLogService] Invalid email: {request.Email}"
                );
            }

            if (!player.IsVerified)
            {
                throw new AppException(
                    userMessage: "Account not verified. Please check your email",
                    statusCode: HttpStatusCode.Forbidden,
                    logMessage: $"[RegVerLogService] Account not verified: {player}"
                );
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, player.PasswordHash))
            {
                throw new AppException(
                    userMessage: "Invalid password",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[RegVerLogService] Invalid password: {request.Password}"
                );
            }

            var playerProfileLogDTO = new PlayerProfileLogDTO
            {
                Id = player.Id,
                Nickname = player.Nickname,
                Level = player.Level,
                Gold = player.Gold,
                Experience = player.Experience,
                AvatarPath = player.AvatarPath,
                BattlesPlayed = player.BattlesPlayed
            };

            return playerProfileLogDTO;
        }

        public async Task<bool> ResetPassword(RessPassRequest request)
        {
            var player = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (player == null)
            {
                throw new AppException(
                    userMessage: "Profile not found",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[RegVerLogService] Profile not found: {player.Email}"
                );
            }

            if (!player.IsVerified)
            {
                throw new AppException(
                    userMessage: "Profile is not verified",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[RegVerLogService] Profile is not verified: {player}"
                );
            }

            string resetCode = Guid.NewGuid().ToString().Substring(0, 6);

            player.PasswordResetCode = resetCode;
            await _context.SaveChangesAsync();

            var emailResult = await SendEmailAsync(request.Email, resetCode, 2);

            if (!emailResult)
            {
                throw new AppException(
                    userMessage: "Failed to send email",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[RegVerLogService] Failed to send email: {emailResult}"
                );
            }

            return true;
        }

        public async Task<bool> VerifyResetPassword(VerRessPassRequest request)
        {
            bool playerExists = await _context.PlayerProfiles.AnyAsync(m => m.Email == request.Email);

            if (!playerExists)
            {
                throw new AppException(
                    userMessage: "Invalid playerId",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[CardService] Invalid playerId: {request.Email}"
                );
            }

            var player = await _context.PlayerProfiles.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (player.PasswordResetCode != request.PasswordResetCode)
            {
                throw new AppException(
                    userMessage: "Invalid verification code",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[RegVerLogService] Invalid verification code: {request.PasswordResetCode}"
                );
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            player.PasswordHash = passwordHash;
            player.PasswordResetCode = "CHANGED";

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
