using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
using TcgPlatformApi.Models;
using TcgPlatformApi.Services;

namespace TcgPlatformApi.Controllers
{
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
        public async Task<IActionResult> AddCardsAsync([FromBody] List<PlayerCardRequest> requests)
        {
            try
            {
                var result = await _playerCardService.AddCardsAsync(requests);
                return Ok("Cards added!");
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

        [HttpGet("getallcards")]
        public async Task<IActionResult> GetPlayerCardsAsync(int playerId)
        {
            try
            {
                var result = await _playerCardService.GetPlayerCardsAsync(playerId);
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

        [HttpPost("removecards")]
        public async Task<IActionResult> RemoveCardsAsync([FromBody] List<PlayerCardRequest> requests)
        {
            try
            {
                var result = await _playerCardService.RemoveCardsAsync(requests);
                return Ok("Cards Removed!");
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
