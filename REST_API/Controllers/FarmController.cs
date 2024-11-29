using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST_API.Models;
using REST_API.Db;
using REST_API.DTOs;
using System.Threading.Tasks;
using REST_API.Services;

using System.Security.Claims;

namespace REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FarmController : ControllerBase
    {
        private readonly FarmService _farmService;

        public FarmController(FarmService farmService)
        {
            _farmService = farmService;
        }

        private int GetFarmManagerIdFromToken()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // FarmManagerId 클레임
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFarm(int id)
        {
            var success = await _farmService.DeleteFarmAsync(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent(); // 204 No Content
        }

        // PUT: api/farm/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFarm(int id, [FromBody] Farm updatedFarm)
        {
            if (id != updatedFarm.Id)
            {
                return BadRequest("Farm ID mismatch");
            }

            var updated = await _farmService.UpdateFarmAsync(id, updatedFarm);

            if (updated == null)
            {
                return NotFound();
            }

            return NoContent(); // 204 No Content

        }
    }

}
