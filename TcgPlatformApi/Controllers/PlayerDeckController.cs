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

        [HttpPost("adddecks")]
        public async Task<IActionResult> AddDeckAsync([FromBody] List<PlayerDeckRequest> requests)
        {
            try
            {
                var result = await _playerDeckService.AddDeckAsync(requests);
                return Ok("Decks added!");
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("removedecks")]
        public async Task<IActionResult> RemoveDeckAsync([FromBody] List<PlayerDeckRemoveRequest> requests) 
        {
            try
            {
                var result = await _playerDeckService.RemoveDeckAsync(requests);
                return Ok("Decks removed!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getdeck")]
        public async Task<IActionResult> GetDeckAsync(int deckId)
        {
            try
            {
                var result = await _playerDeckService.GetDeckAsync(deckId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("getdecks")]
        public async Task<IActionResult> GetDecksByPlayerIdAsync(int playerId)
        {
            try
            {
                var result = await _playerDeckService.GetDecksByPlayerIdAsync(playerId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
