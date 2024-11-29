using REST_API.DTOs;
using REST_API.Models;
using REST_API.Db;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REST_API.Services
{
    public class FarmSaleOrderService
    {
        private readonly ApplicationDbContext _context;

        public FarmSaleOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 구매 요청 생성
        public async Task<FarmSaleOrder> CreateFarmSaleOrderAsync(int farmSaleOfferId, int userId)
        {
            // FarmSalesOffer를 데이터베이스에서 조회
            var farmSalesOffer = await _context.FarmSaleOffers.FindAsync(farmSaleOfferId);
            if (farmSalesOffer == null)
            {
                throw new Exception("FarmSalesOffer not found");
            }

            // 새로운 FarmSaleOrder 생성
            var farmSaleOrder = new FarmSaleOrder
            {
                FarmSaleOfferId = farmSaleOfferId,
                UserId = userId,
                ApprovalStatus = "Pending", // 초기 상태를 '대기 중'으로 설정
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // FarmSaleOrder를 데이터베이스에 추가
            await _context.FarmSaleOrders.AddAsync(farmSaleOrder);
            await _context.SaveChangesAsync();

            return farmSaleOrder;
        }

        // 구매 요청 승인 처리
        public async Task ApproveFarmSaleOrderAsync(int farmSaleOrderId, int sellerId)
        {
            // FarmSaleOrder를 데이터베이스에서 조회
            var farmSaleOrder = await _context.FarmSaleOrders.FindAsync(farmSaleOrderId);
            if (farmSaleOrder == null)
            {
                throw new Exception("FarmSaleOrder not found");
            }

            // 판매자의 정보를 가져오기 위해 FarmSalesOffer를 가져옴
            var farmSalesOffer = await _context.FarmSaleOffers
                .Include(f => f.FarmUnit)
                .FirstOrDefaultAsync(f => f.Id == farmSaleOrder.FarmSaleOfferId);

            if (farmSalesOffer == null)
            {
                throw new Exception("FarmSalesOffer not found");
            }

            // 판매자와 구매자가 동일하지 않은지 확인
            if (farmSalesOffer.FarmUnit.UserId != sellerId)
            {
                throw new Exception("You are not the owner of this sale offer.");
            }

            // 구매자와 판매자의 User 정보를 가져옴
            var buyer = await _context.Users.FindAsync(farmSaleOrder.UserId);
            var seller = await _context.Users.FindAsync(farmSalesOffer.FarmUnit.UserId);

            if (buyer == null || seller == null)
            {
                throw new Exception("Buyer or seller not found.");
            }

            // 가격에 따라 구매자와 판매자의 deposit 업데이트
            var transactionAmount = farmSalesOffer.SuggestedPrice;
            if (buyer.DepositAmount < transactionAmount)
            {
                throw new Exception("Buyer has insufficient funds.");
            }

            buyer.DepositAmount -= transactionAmount;  // 구매자의 예치금 차감
            seller.DepositAmount += transactionAmount; // 판매자의 예치금 증가

            // 승인 상태 업데이트
            farmSaleOrder.ApprovalStatus = "Approved";
            farmSaleOrder.UpdatedAt = DateTime.UtcNow;

            farmSalesOffer.TransactionStatus = "Completed";
            farmSalesOffer.UpdatedAt = DateTime.UtcNow;

            // 데이터베이스에 저장
            _context.Users.Update(buyer);
            _context.Users.Update(seller);
            _context.FarmSaleOrders.Update(farmSaleOrder);
            _context.FarmSaleOffers.Update(farmSalesOffer);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteFarmSaleOrderAsync(int orderId)
        {
            var farmSaleOrder = await _context.FarmSaleOrders.FindAsync(orderId);
            if (farmSaleOrder == null)
            {
                throw new KeyNotFoundException("FarmSaleOrder not found");
            }

            _context.FarmSaleOrders.Remove(farmSaleOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FarmSaleOrder>> GetFarmSaleOrdersByUserAsync(int userId, string approvalStatus = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.FarmSaleOrders
                .Where(order => order.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(approvalStatus))
            {
                query = query.Where(order => order.ApprovalStatus == approvalStatus);
            }

            if (startDate.HasValue)
            {
                query = query.Where(order => order.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(order => order.CreatedAt <= endDate.Value);
            }

            return await query.ToListAsync();
        }
    }
}
