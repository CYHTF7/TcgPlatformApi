using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerCardController : ControllerBase
    {
        private readonly IPlayerCardService _playerCardService;

        public PlayerCardController(IPlayerCardService playerCardService)
        {
            _playerCardService = playerCardService;
        }

        [HttpPost("addcards")]
        public async Task<IActionResult> AddCardsAsync([FromBody] List<CardRequest> requests)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var playerRequests = requests.Select(r => new PlayerCardRequest
            {
                PlayerId = parsedPlayerId,
                CardId = r.CardId,
                Quantity = r.Quantity
            }).ToList();

            var result = await _playerCardService.AddCardsAsync(playerRequests);
            return Ok("Cards added!");
        }

        [HttpGet("getallcards")]
        public async Task<IActionResult> GetPlayerCardsAsync()
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var result = await _playerCardService.GetPlayerCardsAsync(parsedPlayerId);
            return Ok(result);
        }

        [HttpPost("removecards")]
        public async Task<IActionResult> RemoveCardsAsync([FromBody] List<CardRequest> requests)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var playerRequests = requests.Select(r => new PlayerCardRequest
            {
                PlayerId = parsedPlayerId,
                CardId = r.CardId,
                Quantity = r.Quantity
            }).ToList();

            var result = await _playerCardService.RemoveCardsAsync(playerRequests);
            return Ok("Cards Removed!");
        }
    }
}
