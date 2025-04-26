using System.ComponentModel.DataAnnotations;

namespace Master.Models
{
    public class RecyclingRequestVm
    {
        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "اسم العنصر")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "نوع العنصر")]
        public string ItemType { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [Range(1, 100, ErrorMessage = "يجب أن تكون بين 1 و 100")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "حالة العنصر")]
        public string Condition { get; set; }

        [Display(Name = "ملاحظات")]
        [MaxLength(500, ErrorMessage = "لا تتجاوز 500 حرف")]
        public string Notes { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [FutureDate(ErrorMessage = "يجب أن يكون تاريخًا مستقبليًا")]
        [Display(Name = "تاريخ التسليم")]
        public DateTime DeliveryDate { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "وقت التسليم")]
        public TimeSpan DeliveryTime { get; set; } = new TimeSpan(12, 0, 0);

        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "المحافظة")]
        public string City { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "خط العرض")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [Display(Name = "خط الطول")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "اسم المكان مطلوب")]
        [Display(Name = "اسم المكان")]
        public string LocationName { get; set; } // بدلاً من Latitude/Longitude

        [Display(Name = "تسليم سريع")]
        public bool IsExpress { get; set; }

        [RequiredIf(nameof(IsExpress), true, ErrorMessage = "حقل مطلوب للتسليم السريع")]
        [Display(Name = "طريقة الدفع")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "حقل مطلوب")]
        [ImageFile(ErrorMessage = "يجب أن تكون صورة (jpg, png) ولا تتجاوز 5MB")]
        [Display(Name = "صورة العنصر")]
        public IFormFile Image { get; set; }
    }
}
