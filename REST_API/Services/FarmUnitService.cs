using REST_API.Models;
using REST_API.DTOs;
using REST_API.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace REST_API.Services
{
    public class FarmUnitService
    {
        private readonly ApplicationDbContext _context;

        public FarmUnitService(ApplicationDbContext context)
        {
            _context = context;
        }


        // farm unit 유닛 생성 로직
        public async Task<FarmUnit> AddFarmUnitAsync(int farmId, FarmUnitCreateDto farmUnitCreateDto)
        {
            // Farm이 존재하는지 확인
            var farm = await _context.Farms.FirstOrDefaultAsync(f => f.Id == farmId);
            if (farm == null)
            {
                throw new KeyNotFoundException("Farm not found.");
            }

            // FarmUnit 생성
            var farmUnit = new FarmUnit
            {
                Name = farmUnitCreateDto.Name,
                FarmUnitPrice = farmUnitCreateDto.FarmUnitPrice,
                FarmUnitTypeId = farmUnitCreateDto.FarmUnitTypeId,
                Description = farmUnitCreateDto.Description,
                FarmId = farmId,  // Farm과 연결
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // FarmUnit 추가
            _context.FarmUnits.Add(farmUnit);
            await _context.SaveChangesAsync();

            return farmUnit;
        }


        // farm unit 유닛 조회 로직
        public async Task<List<FarmUnit>> GetFarmUnitsByFarmIdAsync(int farmId)
        {
            var farmUnits = await _context.FarmUnits
                                 .Where(fu => fu.FarmId == farmId)
                                 .ToListAsync();
            return farmUnits;
        }


        // 특정 FarmUnit 조회
        public async Task<FarmUnit> GetFarmUnitByFarmUnitIdAsync(int farmUnitId)
        {
            return await _context.FarmUnits.FirstOrDefaultAsync(fu => fu.Id == farmUnitId);
        }

        public async Task<List<FarmUnit>> GetFarmUnitsByUserIdAsync(int userId)
        {
            // 유저 ID에 해당하는 FarmUnit들을 조회
            var farmUnits = await _context.FarmUnits
                .Where(f => f.UserId == userId)
                .ToListAsync();

            return farmUnits;
        }

        public async Task<List<FarmSaleOrder>> GetFarmSaleOrdersByUserAsync(int userId, string? approvalStatus = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // 기본 쿼리: 해당 유저의 FarmSaleOrder를 조회
            var query = _context.FarmSaleOrders
                .Where(order => order.UserId == userId)
                .AsQueryable();

            // 승인 상태에 따른 필터링
            if (!string.IsNullOrEmpty(approvalStatus))
            {
                query = query.Where(order => order.ApprovalStatus == approvalStatus);
            }

            // 날짜 범위에 따른 필터링
            if (startDate.HasValue)
            {
                query = query.Where(order => order.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(order => order.CreatedAt <= endDate.Value);
            }

            // 결과 반환
            return await query.ToListAsync();
        }

    }
}
