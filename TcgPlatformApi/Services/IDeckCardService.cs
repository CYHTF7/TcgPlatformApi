using TcgPlatformApi.Models;

namespace TcgPlatformApi.Services
{
    public interface IDeckCardService
    {
        Task<bool> AddCardInDeckAsync(DeckCardRequest request);
        Task<bool> RemoveCardFromDeckAsync(DeckCardRequest request);
    }
}
