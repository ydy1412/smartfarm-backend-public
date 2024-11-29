using REST_API.Models;
using REST_API.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using REST_API.DTOs;

namespace REST_API.Services
{
    public class FarmSaleOfferService
    {
        private readonly ApplicationDbContext _context;

        public FarmSaleOfferService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateFarmSaleOfferAsync(int farmUnitId)
        {
            // FarmUnit을 비동기로 데이터베이스에서 조회
            var farmUnit = await _context.FarmUnits.FindAsync(farmUnitId);

            if (farmUnit == null)
            {
                throw new Exception("FarmUnit not found");
            }

            // 새로운 FarmSalesOffer 생성
            var farmSalesOffer = new FarmSaleOffer
            {
                FarmUnitId = farmUnitId,
                SuggestedPrice = farmUnit.FarmUnitPrice, // FarmUnit의 가격을 SuggestedPrice로 설정
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // FarmSalesOffer를 데이터베이스에 추가
            await _context.FarmSaleOffers.AddAsync(farmSalesOffer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FarmSaleOffer>> GetFarmSaleOffersByUserAsync(int userId)
        {
            var farmSalesOffers = await _context.FarmSaleOffers
                .Where(offer => offer.FarmUnit.UserId == userId) // FarmUnit의 소유자(UserId)를 기준으로 필터링
                .ToListAsync();

            return farmSalesOffers;
        }

        // 특정 FarmSalesOffer 조회
        public async Task<FarmSaleOffer> GetFarmSaleOfferByIdAsync(int id)
        {
            return await _context.FarmSaleOffers
                .Include(fso => fso.FarmUnit)
                .FirstOrDefaultAsync(fso => fso.Id == id);
        }

        public async Task<List<FarmSaleOffer>> GetFarmSaleOffersByFarmManagerIdAsync(int farmManagerId)
        {
            return await _context.FarmSaleOffers
                .Include(fso => fso.FarmUnit)
                .ThenInclude(fu => fu.Farm)
                .Where(fso => fso.FarmUnit.Farm.FarmManagerId == farmManagerId) // FarmManager와 연관된 FarmUnit을 필터링
                .ToListAsync();
        }

        // 판매 오퍼 업데이트
        public async Task<FarmSaleOffer> UpdateFarmSaleOfferAsync(int farmSalesOfferId, FarmSaleOfferUpdateDto farmSalesOfferUpdateDto)
        {
            var farmSalesOffer = await _context.FarmSaleOffers.FirstOrDefaultAsync(fso => fso.Id == farmSalesOfferId);

            if (farmSalesOffer == null)
            {
                throw new KeyNotFoundException("FarmSalesOffer not found.");
            }

            // 거래 상태 업데이트
            farmSalesOffer.TransactionStatus = farmSalesOfferUpdateDto.transactionStatus;
            farmSalesOffer.SuggestedPrice = farmSalesOfferUpdateDto.suggestedPrice;
            farmSalesOffer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return farmSalesOffer;
        }

        public async Task<bool> DeleteFarmSaleOfferAsync(int farmSalesOfferId)
        {
            var FarmSalesOffer = await _context.FarmSaleOffers.FirstOrDefaultAsync(fso => fso.Id == farmSalesOfferId);
            if (FarmSalesOffer == null)
            {
                throw new KeyNotFoundException("FarmManager not found.");
            }

            _context.FarmSaleOffers.Remove(FarmSalesOffer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}