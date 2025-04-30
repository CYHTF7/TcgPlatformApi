using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IRegVerLogService
    {
        Task<PlayerProfileRegResponse> Register(RegRequest request);
        Task<bool> VerifyAccount(VerRequest request);
        Task<PlayerProfileTokenLogResponse> Login(PlayerProfileLogRequest request);
        Task<bool> ResetPassword(RessPassRequest request);
        Task<bool> VerifyResetPassword(VerRessPassRequest request);
    }
}
