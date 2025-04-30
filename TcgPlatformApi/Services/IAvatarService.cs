namespace TcgPlatformApi.Services
{
    public interface IAvatarService
    {
        Task<string> UploadAvatar(IFormFile file, int playerId);
        Task<byte[]> GetAvatar(int playerId);
    }
}
