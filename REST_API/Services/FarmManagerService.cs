using REST_API.DTOs;
using REST_API.Models;
using REST_API.Db;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REST_API.Services
{
    public class FarmManagerService
    {
        private readonly ApplicationDbContext _context;

        public FarmManagerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FarmManager> CreateFarmManagerAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if (user.IsFarmManager)
            {
                throw new InvalidOperationException("User is already a farm manager.");
            }

            var farmManager = new FarmManager
            {
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.FarmManagers.Add(farmManager);
            user.IsFarmManager = true;
            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return farmManager;
        }


        // 특정 FarmManager의 Farm 목록 조회
        public async Task<List<Farm>> GetFarmsByManagerIdAsync(int farmManagerId)
        {
            return await _context.Farms
                                 .Where(f => f.FarmManagerId == farmManagerId)
                                 .ToListAsync();
        }

        // FarmManager가 관리하는 특정 Farm 조회

        // FarmManager 정보 조회
        public async Task<FarmManager> GetFarmManagerByIdAsync(int farmManagerId)
        {
            return await _context.FarmManagers.FirstOrDefaultAsync(fm => fm.Id == farmManagerId);
        }

        // FarmManager 삭제
        public async Task<bool> DeleteFarmManagerAsync(int farmManagerId)
        {
            var farmManager = await _context.FarmManagers.FirstOrDefaultAsync(fm => fm.Id == farmManagerId);
            if (farmManager == null)
            {
                throw new KeyNotFoundException("FarmManager not found.");
            }

            // User 정보 업데이트 로직 추가
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == farmManagerId);
            if (user != null)
            {
                user.IsFarmManager = false; // 예시로 FarmManager 상태를 해제하는 로직
                _context.Users.Update(user); // User 정보 업데이트
            }

            _context.FarmManagers.Remove(farmManager);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
