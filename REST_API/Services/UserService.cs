using System.Threading.Tasks;
using REST_API.Models;
using Microsoft.EntityFrameworkCore;
using REST_API.Db;
using System;
using Microsoft.AspNetCore.Identity;


namespace REST_API.Services
{
    public class UserService
    {

        private readonly ApplicationDbContext _context;

        private readonly AuthService _authService;

        public UserService(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<String> AuthenticateAsync(string username, string password)
        {
            // 데이터베이스에서 사용자 찾기
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Name == username);
            if (user == null)
            {
                return null; // 사용자가 없으면 null 반환
            }

            // 비밀번호 검증
            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null; // 비밀번호가 일치하지 않음
            }

             var token = _authService.GenerateJwtToken(user);

            // 인증 성공, 사용자 반환
            return token;
        }

        // 유저 등록 메서드
        public async Task<User> RegisterUserAsync(string name, string address, string phoneNumber, string password, decimal depositAmount)
        {
            if (await _context.Users.AnyAsync(u => u.Name == name))
            {
                throw new ArgumentException("A user with this name already exists.");
            }

            var user = new User
            {
                Name = name,
                Address = address,
                PhoneNumber = phoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(password), // 비밀번호 해싱
                DepositAmount = depositAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        // 유저 삭제 메서드

        // 유저 수정 메서드
        public async Task<User> EditUserAsync(int userId, string name, string address, string phoneNumber, decimal depositAmount)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // 필요한 필드 수정
            user.Name = name;
            user.Address = address;
            user.PhoneNumber = phoneNumber;
            user.DepositAmount = depositAmount;
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return user;
        }

        // 비밀번호 변경 메서드

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            // 현재 비밀번호가 일치하는지 확인
            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            // 새로운 비밀번호 해싱 후 저장
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        // 유저 조회 메서드
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }


        // 농장 매니저 삭제
        public async Task DeleteFarmManagerAsync(int farmManagerId)
        {
            var farmManager = await _context.FarmManagers.FindAsync(farmManagerId);
            if (farmManager == null)
            {
                throw new KeyNotFoundException("Farm Manager not found.");
            }

            var user = await _context.Users.FindAsync(farmManager.UserId);
            if (user != null)
            {
                user.IsFarmManager = false;
                _context.Users.Update(user);
            }

            _context.FarmManagers.Remove(farmManager);
            await _context.SaveChangesAsync();
        }

        // 농장 매니저 수정
        public async Task<FarmManager> EditFarmManagerAsync(int farmManagerId, DateTime updatedCreatedAt)
        {
            var farmManager = await _context.FarmManagers.FindAsync(farmManagerId);
            if (farmManager == null)
            {
                throw new KeyNotFoundException("Farm Manager not found.");
            }

            farmManager.CreatedAt = updatedCreatedAt;
            farmManager.UpdatedAt = DateTime.UtcNow;

            _context.FarmManagers.Update(farmManager);
            await _context.SaveChangesAsync();

            return farmManager;
        }
    }
}
