namespace Master.Models
{
    public class TempCartItem
    {
        // فقط الخصائص الضرورية لعرض المنتج
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; } // (UnitPrice)

        // يمكنك إضافة هذه الدالة لحساب السعر الكلي
        public decimal TotalPrice => Price * Quantity;
    }
}
