using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TcgPlatformApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BoosterController : ControllerBase
    {
        private readonly IBoosterService _playerBoosterService;

        public BoosterController(IBoosterService playerBoosterService)
        {
            _playerBoosterService = playerBoosterService;
        }

        [HttpPost("addboosters")]
        public async Task<IActionResult> AddBoosterAsync([FromBody] List<BoosterRequest> requests)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var playerRequests = requests.Select(r => new PlayerBoosterRequest
            {
                PlayerId = parsedPlayerId,
                BoosterId = r.BoosterId,
                Quantity = r.Quantity
            }).ToList();

            var result = await _playerBoosterService.AddBoosterAsync(playerRequests);
            return Ok("Booster added!");
        }

        [HttpGet("getallboosters")]
        public async Task<IActionResult> GetPlayerBoostersAsync()
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var result = await _playerBoosterService.GetPlayerBoostersAsync(parsedPlayerId);
            return Ok(result);
        }


        [HttpPost("removeboosters")]
        public async Task<IActionResult> RemoveBoosterAsync([FromBody] List<BoosterRequest> requests)
        {
            var playerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(playerId, out int parsedPlayerId))
            {
                return BadRequest("Invalid playerId!");
            }

            var playerRequests = requests.Select(r => new PlayerBoosterRequest
            {
                PlayerId = parsedPlayerId,
                BoosterId = r.BoosterId,
                Quantity = r.Quantity
            }).ToList();

            var result = await _playerBoosterService.RemoveBoosterAsync(playerRequests);
            return Ok("Booster removed!");
        }
    }
}
