using Master.Models;

namespace Master.Extensions
{
    public class EcoPointsService
    {
        private readonly MyDBContext _db;

        public EcoPointsService(MyDBContext db)
        {
            _db = db;
        }

        public async Task<Coupon?> RedeemPoints(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null || user.Points < 100) return null;

            // خصم 100 نقطة وإنشاء كوبون جديد
            user.Points -= 100;

            var coupon = new Coupon
            {
                Code = GenerateCouponCode(),
                DiscountPercentage = 15.00m, // خصم 15%
                ExpiryDate = DateTime.Now.AddMonths(1), // ينتهي بعد شهر
                IsActive = true
            };

            _db.Coupons.Add(coupon);
            await _db.SaveChangesAsync();

            return coupon;
        }

        private string GenerateCouponCode()
        {
            return "ECO-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
        }

        public int GetRemainingPointsForNextReward(int currentPoints)
        {
            return 100 - (currentPoints % 100);
        }
    }
}
