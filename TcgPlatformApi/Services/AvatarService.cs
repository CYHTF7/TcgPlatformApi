using Microsoft.AspNetCore.Mvc;
using System.Net;
using TcgPlatformApi.Data;
using TcgPlatformApi.Exceptions;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public class AvatarService : IAvatarService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly string _avatarsFolder;

        public AvatarService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            _avatarsFolder = Path.Combine(_env.WebRootPath, "avatars");

            if (!Directory.Exists(_avatarsFolder))
            {
                Directory.CreateDirectory(_avatarsFolder);
            }
        }

        public async Task<string> UploadAvatar(IFormFile file, int playerId)
        {
            if (file == null || file.Length == 0)
            {
                throw new AppException(
                    userMessage: "Invalid file",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[AvatarService] Invalid file {file.Length}"
                );
            }

            var allovedTypes = new[] { "image/jpeg", "image/pjpeg", "image/jpg" };
            if (!allovedTypes.Contains(file.ContentType))
            {
                throw new AppException(
                    userMessage: "File must be .jpg",
                    statusCode: HttpStatusCode.BadRequest,
                    logMessage: $"[AvatarService] File must be .jpg: {file.FileName}"
                );
            }

            var user = await _context.PlayerProfiles.FindAsync(playerId);
            if (user == null)
            {
                throw new AppException(
                    userMessage: "Account not found!",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[AvatarService] Account not found for playerId: {playerId}"
                );
            }

            string fileName = $"{playerId}.jpg";
            string filePath = Path.Combine(_avatarsFolder, fileName);

            DeleteOldAvatar(filePath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.AvatarPath = $"/avatars/{fileName}";
            await _context.SaveChangesAsync();

            return user.AvatarPath;
        }

        private void DeleteOldAvatar(string filePatch) 
        {
            if (string.IsNullOrEmpty(filePatch)) return;

            string oldFilePath = Path.Combine(_env.WebRootPath, filePatch.TrimStart('/'));

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
        }

        public async Task<byte[]> GetAvatar(int playerId)
        {
            var user = await _context.PlayerProfiles.FindAsync(playerId);
            if (user?.AvatarPath == null)
            {
                throw new AppException(
                    userMessage: "Avatar not found",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[AvatarService] Avatar not found for playerId: {playerId}"
                );
            }

            string filePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                throw new AppException(
                    userMessage: "Avatar is missing",
                    statusCode: HttpStatusCode.NotFound,
                    logMessage: $"[AvatarService] Avatar is missing for playerId: {playerId}"
                );
            }

            return await System.IO.File.ReadAllBytesAsync(filePath);
        }
    }
}
