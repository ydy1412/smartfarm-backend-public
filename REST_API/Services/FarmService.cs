using REST_API.Models;
using REST_API.DTOs;

using Microsoft.EntityFrameworkCore;
using REST_API.Db;

namespace REST_API.Services
{
    public class FarmService
    {
        private readonly ApplicationDbContext _context;

        public FarmService(ApplicationDbContext context)
        {
            _context = context;
        }

        // FarmUnit 조회
        public async Task<Farm> CreateFarmAsync(int farmManagerId, FarmCreateDto farmCreateDto)
        {
            var farmManager = await _context.FarmManagers.FirstOrDefaultAsync(fm => fm.Id == farmManagerId);
            if (farmManager == null)
            {
                throw new KeyNotFoundException("FarmManager not found.");
            }

            // FarmManager와 Farm 연결
            var farm = new Farm
            {
                Name = farmCreateDto.name,
                FarmManagerId = farmManagerId,  // farmManagerId를 DTO에서 넘어온 farmManagerId로 설정
                IpAddress = farmCreateDto.ipAddress,
                Address = farmCreateDto.address,
                Description = farmCreateDto.description
            };


            // Farm 추가
            _context.Farms.Add(farm);
            await _context.SaveChangesAsync();

            return farm;
        }

        public async Task<List<Farm>> GetFarmsByManagerIdAsync(int farmManagerId)
        {
            return await _context.Farms
                .Where(f => f.FarmManagerId == farmManagerId)
                .ToListAsync();
        }

        public async Task<List<Farm>> GetFarmsWithUnitsByManagerIdAsync(int farmManagerId)
        {
            return await _context.Farms
                .Include(f => f.FarmUnits) // Farm과 연결된 FarmUnit을 함께 로드
                .Where(f => f.FarmManagerId == farmManagerId)
                .ToListAsync();
        }
        
        // 특정 Farm 조회
        public async Task<Farm> GetFarmByIdAsync(int farmId)
        {
            return await _context.Farms.FirstOrDefaultAsync(f => f.Id == farmId);
        }

        // 모든 Farm 조회
        public async Task<List<Farm>> GetAllFarmsAsync()
        {
            return await _context.Farms.ToListAsync();
        }

        // Farm 업데이트
        public async Task<Farm> UpdateFarmAsync(int farmId, Farm updatedFarm)
        {
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == farmId);

            if (farm == null)
            {
                throw new KeyNotFoundException("Farm not found.");
            }

            // 기존 Farm의 속성 업데이트
            farm.Name = updatedFarm.Name;
            farm.Address = updatedFarm.Address;
            farm.Description = updatedFarm.Description;
            farm.UpdatedAt = DateTime.UtcNow; // 업데이트 시간 갱신

            await _context.SaveChangesAsync();
            return farm;
        }

        // Farm 삭제
        public async Task<bool> DeleteFarmAsync(int farmId)
        {
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == farmId);

            if (farm == null)
            {
                throw new KeyNotFoundException("Farm not found.");
            }

            _context.Farms.Remove(farm);
            await _context.SaveChangesAsync();

            return true; // 성공적으로 삭제되면 true 반환
        }

    }


}
