using Master.Models;
using System.Drawing;
using System;
using Microsoft.AspNetCore.Mvc;

namespace Master.Controllers
{
    public class RecyclingController : Controller
    {
        private readonly MyDBContext _db;
        private readonly IWebHostEnvironment _env;

        public RecyclingController(MyDBContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        //---------------------------------------------create recycling request-------------------------------

        [HttpGet]
        public IActionResult Create()
        {
            return View(new RecyclingRequestVm());
        }

        [HttpPost]
        public async Task<IActionResult> Create(RecyclingRequestVm model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var points = 0;

            try
            {
                var requestedDateTime = model.DeliveryDate.Add(model.DeliveryTime);

                // تحقق من وجود حجز بنفس المدينة وضمن ساعة واحدة من الوقت المطلوب
                // 🔥 تحقق فقط لو مش تسليم سريع
                if (!model.IsExpress)
                {
                    bool isSlotTaken = _db.RecyclingRequests
                        .Any(r =>
                            r.City == model.City &&
                            r.RequestedDate >= requestedDateTime &&
                            r.RequestedDate <= requestedDateTime.AddHours(1)
                        );

                    if (isSlotTaken)
                    {
                        return Json(new { success = false, error = "عذراً، هناك حجز آخر خلال ساعة من هذا الوقت في نفس المدينة. الرجاء اختيار وقت آخر." });
                    }
                }


                // حفظ الصورة
                string imagePath = null;
                if (model.Image != null)
                {
                    imagePath = await SaveImage(model.Image);
                }
                var paymentId = -1;
                // معالجة الدفع السريع
                if (model.IsExpress)
                {
                    var payment = new Payment
                    {
                        Amount = 3.00m,
                        PaymentMethod = model.PaymentMethod,
                        Status = (model.PaymentMethod == "cash")? "بانتظار الدفع": "تم الدفع",
                        CreatedAt = DateTime.Now,
                        PaymentType = "Recycle",
                        UserId = userId
                    };
                    _db.Payments.Add(payment);
                    await _db.SaveChangesAsync();
                    paymentId = payment.Id;

                    points += 30;
                }
                // إنشاء الطلب
                var request = new RecyclingRequest
                {
                    MaterialName = model.ItemName,
                    MaterialType = model.ItemType,
                    Quantity = model.Quantity,
                    ItemCondition = model.Condition,
                    Notes = model.Notes,
                    RequestedDate = requestedDateTime,
                    City = model.City,
                    Location = model.LocationName,
                    ImageUrl = imagePath,
                    CreatedAt = DateTime.Now,
                    UserId = userId,
                    PaymentId = paymentId>-1 ? paymentId: null,
                    DeliveryStatus = false,
                    Longitude= (decimal?)model.Longitude,
                    Latitude= (decimal?)model.Latitude
                };

                points += model.Quantity * 2;




                _db.RecyclingRequests.Add(request);
                await _db.SaveChangesAsync();

                var user = _db.Users.Find(userId);
                user.Points = user.Points + points;
                _db.Users.Update(user);
                await _db.SaveChangesAsync();

                return Json(new { success = true, redirect = Url.Action("Index", "Home") });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }


        private async Task<string> SaveImage(IFormFile image)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }
    }
}
