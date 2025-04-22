using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Data;

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

        [HttpPost("upload")]
        public async Task<string> UploadAvatar(IFormFile file, int userId)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file!");
            }

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/jpg")
            {
                throw new ArgumentException("File is not .jpg!");
            }

            var user = await _context.PlayerProfiles.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("Account not found!");
            }

            string fileName = $"{userId}.jpg";
            string filePath = Path.Combine(_avatarsFolder, fileName);

            if (!string.IsNullOrEmpty(user.AvatarPath))
            {
                string oldFilePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.AvatarPath = $"/avatars/{fileName}";
            await _context.SaveChangesAsync();

            return user.AvatarPath;
        }

        [HttpGet("get/{userId}")]
        public async Task<byte[]> GetAvatar(int userId)
        {
            var user = await _context.PlayerProfiles.FindAsync(userId);
            if (user?.AvatarPath == null)
            {
                throw new KeyNotFoundException("Avatar not found!");
            }

            string filePath = Path.Combine(_env.WebRootPath, user.AvatarPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("Avatar is missing!");
            }

            return await System.IO.File.ReadAllBytesAsync(filePath);
        }
    }
}
