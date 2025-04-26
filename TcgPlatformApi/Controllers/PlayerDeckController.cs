using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerDeckController : ControllerBase
    {
        private readonly IPlayerDeckService _playerDeckService;

        public PlayerDeckController(IPlayerDeckService playerDeckService)
        {
            _playerDeckService = playerDeckService;
        }

        [HttpPost("addorupdatedecks")]
        public async Task<IActionResult> AddorUpdateDecksAsync([FromBody] List<PlayerDeckRequest> requests)
        {
            var result = await _playerDeckService.AddorUpdateDecksAsync(requests);
            return Ok("Decks added!");
        }

        [HttpPost("removedecks")]
        public async Task<IActionResult> RemoveDeckAsync([FromBody] List<PlayerDeckRemoveRequest> requests) 
        {
            var result = await _playerDeckService.RemoveDeckAsync(requests);
            return Ok("Decks removed!");
        }

        [HttpGet("getdeck")]
        public async Task<IActionResult> GetDeckAsync(int deckId)
        {
            var result = await _playerDeckService.GetDeckAsync(deckId);
            return Ok(result);
        }

        [HttpGet("getdecks")]
        public async Task<IActionResult> GetDecksByPlayerIdAsync(int playerId)
        {
            var result = await _playerDeckService.GetDecksByPlayerIdAsync(playerId);
            return Ok(result);
        }
    }
}
