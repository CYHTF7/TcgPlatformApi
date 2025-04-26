using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IRegVerLogService
    {
        Task<PlayerProfileRegDTO> Register(RegRequest request);
        Task<bool> VerifyAccount(VerRequest request);
        Task<PlayerProfileLogDTO> Login(LogRequest request);
        Task<bool> ResetPassword(RessPassRequest request);
        Task<bool> VerifyResetPassword(VerRessPassRequest request);
    }
}
