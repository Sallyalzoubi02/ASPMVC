namespace Master.Models
{
    public class AppliedCoupon
    {
        public string Code { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
