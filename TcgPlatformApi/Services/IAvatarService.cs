using Microsoft.AspNetCore.Mvc;

namespace TcgPlatformApi.Services
{
    public interface IAvatarService
    {
        Task<string> UploadAvatar(IFormFile file, int userId);
        Task<byte[]> GetAvatar(int userId);
    }
}
