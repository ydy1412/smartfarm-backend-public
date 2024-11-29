using Microsoft.AspNetCore.Mvc;
using REST_API.Services;
using REST_API.Models;
using REST_API.DTOs;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace REST_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FarmManagerController : ControllerBase
    {
        private readonly FarmManagerService _farmManagerService;
        private readonly FarmService _farmService;

        private readonly FarmSaleOfferService _farmSalesOfferService;

        private readonly FarmUnitService _farmUnitService;

        private readonly UserService _userService;

        public FarmManagerController(FarmManagerService farmManagerService, FarmService farmService, FarmSaleOfferService farmSalesOfferService, FarmUnitService farmUnitService, UserService userService)
        {
            _farmManagerService = farmManagerService;
            _farmService = farmService;
            _farmSalesOfferService = farmSalesOfferService;
            _farmUnitService = farmUnitService;
            _userService = userService; // UserService 주입
        }

        private int GetUserIdFromToken()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)); // UserId 클레임에서 추출
        }

        private async Task<int> GetManagerIdFromTokenAsync()
        {
            var userId = GetUserIdFromToken(); // 토큰에서 UserId 추출
            var user = await _userService.GetUserByIdAsync(userId); // UserService를 통해 UserId로 User 정보 조회
            if (user == null || user.FarmManager == null)
            {
                throw new Exception("ManagerId not found for this user.");
            }

            return user.FarmManager.Id; // User의 ManagerId 반환
        }

        // GET: api/farmmanager/{farmManagerId}/farms
        [HttpGet("farms")]
        public async Task<IActionResult> GetFarms()
        {
            var farmManagerId = await GetManagerIdFromTokenAsync();
            var farms = await _farmService.GetFarmsWithUnitsByManagerIdAsync(farmManagerId);
            if (farms == null || farms.Count == 0)
            {
                return NotFound(new { message = "No farms found for this FarmManager." });
            }

            return Ok(farms);
        }

        [HttpPost("farms")]
        public async Task<IActionResult> CreateFarm([FromBody] FarmCreateDto farmCreateDto)
        {
            try
            {
                // 토큰에서 ManagerId 추출
                var farmManagerId = await GetManagerIdFromTokenAsync();

                // FarmCreateDto를 통해 농장 생성 요청
                var createdFarm = await _farmService.CreateFarmAsync(farmManagerId, farmCreateDto);


                // 여기에 farm 생성시 추가 로직 추가.
                // 라즈베리파이와 연결 시도 => 라즈베리 파이와 연결 완료시 라즈베리파이 서버에 등록된 모든 정보 가져옴.
                // 그리고 라즈베리파이 서버에 등록되어 있는 아두이노 리스트 ( 팜 유닛 리스트)를 불러온 후 DB에 저장.
                // 이 모든 작업이 완료되면 생성 확인.
                // 각 작업에 대한 오류 정보 출력 필요.

                return CreatedAtAction(nameof(GetFarms), new { farmManagerId = createdFarm.Id }, createdFarm);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("farms/{farmId}/farm_units")]
        public async Task<IActionResult> GetFarmUnits(int farmId)
        {
            
            var farmUnits = await _farmUnitService.GetFarmUnitsByFarmIdAsync(farmId);
            if (farmUnits == null || farmUnits.Count == 0 )
            {
                return NotFound(new { message = "No farms found for this FarmManager." });
            }

            return Ok(farmUnits);
        }

        [HttpPost("{farmManagerId}/farm_sales_offer/{farmUnitId}")]
        public async Task<IActionResult> CreateFarmSalesOffer(int farmUnitId)
        {
            try
            {
                // 비동기로 FarmSalesOffer 생성 메서드 호출
                await _farmSalesOfferService.CreateFarmSaleOfferAsync(farmUnitId);
                return Ok("FarmSalesOffer created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPut("{farmManagerId}/farm_sales_offer/{farmUnitId}")]
        public async Task<IActionResult> UpdateFarmSalesOffer(int farmUnitId, [FromBody] FarmSaleOfferUpdateDto farmSalesOfferUpdteDto)
        {
            try
            {
                // 비동기로 FarmSalesOffer 생성 메서드 호출
                await _farmSalesOfferService.CreateFarmSaleOfferAsync(farmUnitId);
                return Ok("FarmSalesOffer created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpDelete("{farmManagerId}/farm_sales_offer/{farmUnitId}")]
        public async Task<IActionResult> DeleteFarmSalesOffer(int farmUnitId)
        {
            try
            {
                // 비동기로 FarmSalesOffer 생성 메서드 호출
                await _farmSalesOfferService.DeleteFarmSaleOfferAsync(farmUnitId);
                return Ok("FarmSalesOffer deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }

}
