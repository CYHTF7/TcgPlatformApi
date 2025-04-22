using Microsoft.AspNetCore.Mvc;
using TcgPlatformApi.Data;
using TcgPlatformApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TcgPlatformApi.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TcgPlatformApi.Services;
using System.Collections.Generic;

namespace TcgPlatformApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerBoosterController : ControllerBase
    {
        private readonly IPlayerBoosterService _playerBoosterService;

        public PlayerBoosterController(IPlayerBoosterService playerBoosterService)
        {
            _playerBoosterService = playerBoosterService;
        }

        [HttpPost("addboosters")]
        public async Task<IActionResult> AddBoosterAsync([FromBody] List<PlayerBoosterRequest> requests)
        {
            try
            {
                var result = await _playerBoosterService.AddBoosterAsync(requests);
                return Ok("Booster added!");
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

        [HttpGet("getallboosters")]
        public async Task<IActionResult> GetPlayerBoostersAsync(int playerId)
        {
            try
            {
                var result = await _playerBoosterService.GetPlayerBoostersAsync(playerId);
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


        [HttpPost("removeboosters")]
        public async Task<IActionResult> RemoveBoosterAsync([FromBody] List<PlayerBoosterRequest> requests)
        {
            try
            {
                var result = await _playerBoosterService.RemoveBoosterAsync(requests);
                return Ok("Booster removed!");
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
