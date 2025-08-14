using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Authorize]
    [Route("api/deckcard")]
    [ApiController]
    public class DeckCardController : ControllerBase
    {
        private readonly IDeckCardService _deckCardService;

        public DeckCardController(IDeckCardService deckCardService)
        {
            _deckCardService = deckCardService;
        }

        [HttpPost("adddeckcard")]
        public async Task<IActionResult> AddCardInDeckAsync([FromBody] DeckCardRequest request)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var result = await _deckCardService.AddCardInDeckAsync(request);

            return Ok("DeckCard added!");
        }

        [HttpPost("removedeckcard")]
        public async Task<IActionResult> RemoveCardsAsync([FromBody] DeckCardRemoveRequest request)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var result = await _deckCardService.RemoveCardFromDeckAsync(request);

            return Ok("Cards Removed!");
        }

        [HttpPost("updatedeckcardorder")]
        public async Task<IActionResult> UpdateDeckCardOrderAsync([FromBody] DeckCardOrderRequest request)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var result = await _deckCardService.UpdateCardsOrderAsync(request);

            return Ok("Card Order Updated!");
        }
    }
}
