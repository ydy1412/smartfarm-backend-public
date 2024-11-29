using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using REST_API.Services;
using REST_API.Models;
using REST_API.DTOs;

using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace REST_API.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FarmService _farmService;

        private readonly FarmManagerService _farmManagerService;

        private readonly FarmSaleOfferService _farmSalesOfferService;

        private readonly FarmSaleOrderService _farmSaleOrderService;

        public UserController(UserService userService, FarmService farmService, FarmManagerService farmManagerService,
                              FarmSaleOfferService farmSalesOfferService, FarmSaleOrderService farmSaleOrderService
                              )
        {
            _userService = userService;
            _farmService = farmService;
            _farmManagerService = farmManagerService;
            _farmSalesOfferService = farmSalesOfferService;
            _farmSaleOrderService = farmSaleOrderService;
    
        }
        // 유저 등록 엔드포인트
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var createdUser = await _userService.RegisterUserAsync(
                    registerUserDto.Name, 
                    registerUserDto.Address,
                    registerUserDto.PhoneNumber,
                    registerUserDto.Password,
                    registerUserDto.DepositAmount);
                return Ok(createdUser);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetUserIdFromToken()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyUserInfo()
        {
            try
            {
                Console.WriteLine("get my user api called");
                var userId = GetUserIdFromToken();
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // 유저 삭제 (JWT로 보호된 API)
        [HttpDelete("me")]
        public async Task<IActionResult> DeleteMyAccount()
        {
            try
            {
                var userId = GetUserIdFromToken();
                await _userService.DeleteUserAsync(userId);
                return NoContent(); // 204 No Content 반환
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // 유저 수정 (JWT로 보호된 API)
        [HttpPut("me")]
        public async Task<IActionResult> EditMyAccount([FromBody] UserEditDto editDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var updatedUser = await _userService.EditUserAsync(userId, editDto.Name, editDto.Address, editDto.PhoneNumber, editDto.DepositAmount);
                return Ok(updatedUser); // 200 OK 반환
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // 비밀번호 변경 엔드포인트
        [HttpPatch("me/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = GetUserIdFromToken();
                await _userService.ChangePasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                return NoContent(); // 204 No Content 반환
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        //farms 관련 api

        private int GetFarmManagerIdFromToken()
        {
            return int.Parse(User.FindFirstValue("FarmManagerId")); // 토큰에서 FarmManagerId 추출
        }

        [HttpGet("me/farms")]
        public async Task<IActionResult> GetMyFarms()
        {
            try
            {
                // JWT 토큰에서 FarmManagerId를 추출
                var farmManagerId = GetFarmManagerIdFromToken();

                // 해당 FarmManager가 관리하는 Farm 리스트를 가져옴
                var farms = await _farmService.GetFarmsByManagerIdAsync(farmManagerId);

                return Ok(farms); // 200 OK와 함께 Farm 리스트 반환
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        //farm manager 관련 api

        [HttpPost("me/farm-manager")]
        public async Task<IActionResult> CreateFarmManager()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var farmManager = await _farmManagerService.CreateFarmManagerAsync(userId);
                return Ok(farmManager);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}"); // 404 Not Found
            }
        }


        // 2. 농장 매니저 삭제
        [HttpDelete("me/farm-manager")]
        public async Task<IActionResult> DeleteFarmManager()
        {
            try
            {
                var userId = GetUserIdFromToken();
                await _farmManagerService.DeleteFarmManagerAsync(userId);
                return NoContent(); // 204 No Content, 성공적으로 삭제됨
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404 Not Found
            }
        }

        //farm sale order 관련 api
        [HttpGet("me/farm-sale-orders")]
        public async Task<IActionResult> GetFarmSaleOrdersByUser(string approvalStatus = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var userId = GetUserIdFromToken();
                var farmSaleOrders = await _farmSaleOrderService.GetFarmSaleOrdersByUserAsync(userId, approvalStatus, startDate, endDate);

                if (farmSaleOrders == null || !farmSaleOrders.Any())
                {
                    return NotFound("No farm sale orders found for this user.");
                }

                return Ok(farmSaleOrders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("me/farm-sale-orders")]
        public async Task<IActionResult> CreateFarmSaleOrder([FromBody] int farmSalesOfferId)
        {
            try
            {
                // JWT 토큰에서 유저 ID를 추출
                var userId = GetUserIdFromToken();

                // FarmSaleOrder 생성
                var farmSaleOrder = await _farmSaleOrderService.CreateFarmSaleOrderAsync(farmSalesOfferId, userId);

                return Ok(farmSaleOrder); // 생성된 FarmSaleOrder 반환
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("me/farm-sale-orders/{order_id}")]
        public async Task<IActionResult> DeleteFarmSaleOrder(int order_id)
        {
            try
            {
                await _farmSaleOrderService.DeleteFarmSaleOrderAsync(order_id);
                return NoContent(); // 성공적으로 삭제된 경우
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = "FarmSaleOrder not found" }); // 없는 경우
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message }); // 기타 예외 처리
            }
        }

        //farm sale offer 관련 api
        [HttpGet("me/farm-sales-offers")]
        public async Task<IActionResult> GetFarmSalesOffersByUser()
        {
            try
            {
                var userId = GetUserIdFromToken();
                var farmSalesOffers = await _farmSalesOfferService.GetFarmSaleOffersByUserAsync(userId);

                if (farmSalesOffers == null || !farmSalesOffers.Any())
                {
                    return NotFound("No farm sales offers found for this user.");
                }

                return Ok(farmSalesOffers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
