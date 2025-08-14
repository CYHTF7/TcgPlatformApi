using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Authorize]
    [Route("api/deck")]
    [ApiController]
    public class DeckController : ControllerBase
    {
        private readonly IDeckService _playerDeckService;

        public DeckController(IDeckService playerDeckService)
        {
            _playerDeckService = playerDeckService;
        }

        [HttpPost("addorupdatedecks")]
        public async Task<IActionResult> AddorUpdateDecksAsync([FromBody] List<DeckRequest> requests)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var playerRequests = requests.Select(r => new PlayerDeckRequest
            {
                DeckId = r.DeckId,
                DeckName = r.DeckName,
                PlayerId = parsedPlayerId,
                Cards = r.Cards.Select(c => new CardInDeck
                {
                    CardId = c.CardId,
                    Quantity = c.Quantity
                }).ToList()
            }).ToList();

            var result = await _playerDeckService.AddorUpdateDecksAsync(playerRequests);
            return Ok("Decks added!");
        }

        [HttpPost("removedecks")]
        public async Task<IActionResult> RemoveDeckAsync([FromBody] List<DeckRemoveRequest> requests) 
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var playerRequests = requests.Select(r => new PlayerDeckRemoveRequest
            {
                PlayerId = parsedPlayerId,
                DeckId = r.DeckId,
            }).ToList();

            var result = await _playerDeckService.RemoveDeckAsync(playerRequests);
            return Ok("Decks removed!");
        }

        [HttpGet("getdeck")]
        public async Task<IActionResult> GetDeckAsync(int deckId)
        {
            var result = await _playerDeckService.GetDeckAsync(deckId);
            return Ok(result);
        }

        [HttpGet("getalldecks")]
        public async Task<IActionResult> GetAllDecksByPlayerIdAsync()
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var result = await _playerDeckService.GetAllDecksByPlayerIdAsync(parsedPlayerId);
            return Ok(result);
        }
    }
}
