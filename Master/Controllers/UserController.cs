using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Master.Extensions;
using Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Master.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDBContext myDb;
        private readonly EcoPointsService _ecoPointsService;

        public UserController(MyDBContext myDbContext , EcoPointsService ecoPointsService)
        {
            myDb = myDbContext;
            _ecoPointsService = ecoPointsService;

        }

        public IActionResult add()
        {
            return View();
        }

        private async Task<string> SaveUploadedFile(IFormFile file)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/uploads/" + uniqueFileName;
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return Json(new { success = false, message = "لم يتم اختيار ملف" });

                var imagePath = await SaveUploadedFile(file);

                // هنا يمكنك حفظ مسار الصورة في قاعدة البيانات للمستخدم الحالي
                var userId = HttpContext.Session.GetInt32("UserId");

                var user =  myDb.Users.FirstOrDefault(u => u.Id == userId);
                ;
                user.Image = imagePath;

                myDb.Users.Update(user);
                myDb.SaveChanges();

                return Json(new { success = true, imagePath = imagePath });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //private decimal CalculateExpressFee(string city)
        //{
        //    return city switch
        //    {
        //        "Amman" => 5.00m,
        //        "Irbid" => 7.00m,
        //        _ => 10.00m
        //    };
        //}

        public IActionResult profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = myDb.Users
                .Include(u => u.Orders)
                    .ThenInclude(u => u.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(u => u.RecyclingRequests)
                .Include(u => u.Companies)
                .Include(u => u.Coupons)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            // التحقق إذا فيه شركة وبياناتها ناقصة (هنا مثال بسيط)
            bool incompleteCompanyInfo = !(user.Companies.Any(u=> u.OwnerId == user.Id));
            HttpContext.Session.SetString("IncompleteCompany", incompleteCompanyInfo ? "true" : "false");


            var model = new UserProfileViewModel
            {
                User = user,
                Orders = user.Orders.ToList(),
                RecyclingRequests = user.RecyclingRequests.ToList(),
                IsOwner = user.UserType == "company",
                Companies = user.Companies.ToList(),
                // إضافة اشتراكات المستخدم
                ActiveSubscriptions = myDb.Subscriptions
                    .Where(s => s.UserId == userId && s.IsActive)
                    .ToList(),
               UserCoupons = user.Coupons
                    .OrderByDescending(c => c.ExpiryDate)
                    .ToList()
            };

            Console.WriteLine("IncompleteCompany = " + HttpContext.Session.GetString("IncompleteCompany"));

            return View(model);

        }

        [HttpPost]
        public IActionResult UpdateProfile(string Name, DateOnly birth_date, string email, string phone)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var user = myDb.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Name = Name;
                user.Email = email;
                user.Phone = phone;
                user.BirthDate = birth_date;
                myDb.SaveChanges();
                return RedirectToAction("profile");
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateCompany(string CompanyName)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var newCompany = new Company
            {
                CompanyName = CompanyName,
                OwnerId = userId.Value
            };

            myDb.Companies.Add(newCompany);
            myDb.SaveChanges();

            TempData["SuccessMessage"] = "تم إنشاء الشركة بنجاح!";
            return RedirectToAction("Profile");
        }


        [HttpPost]
        public IActionResult UpdateCompany(Company model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var company = myDb.Companies.FirstOrDefault(c => c.Id == model.Id && c.OwnerId == userId);
            if (company == null) return NotFound();

            company.CompanyName = model.CompanyName;
            

            myDb.SaveChanges();

            TempData["SuccessMessage"] = "تم تحديث بيانات الشركة بنجاح!";
            return RedirectToAction("Profile");
        }


        public IActionResult OrderDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            var order = myDb.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("logged");
            HttpContext.Session.Remove("UserId");
            Response.Cookies.Delete("UserId");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Subscribe()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Subscription model)
        {
            // Add validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Auth");

            // Additional business validation
            model.StartDate = DateTime.Now.Date;
            

            // Setting the user ID and default values
            model.UserId = userId.Value;
            model.IsActive = true;

            // If no StartDate is passed, set it to now
            if (model.StartDate == DateTime.MinValue)
            {
                model.StartDate = DateTime.Now;
            }
            if (model.SubscriptionType?.ToLower() == "daily")
            {
                
                model.DayOfWeek = "All";
                
            }
            // Optional: You can add any other validation or logic here for specific fields

            myDb.Subscriptions.Add(model);
            myDb.SaveChanges();

            return RedirectToAction("Profile");
        }


        [HttpPost]
        public IActionResult Cancel(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId == null) return Unauthorized();

                var subscription = myDb.Subscriptions.Find(id);
                if (subscription == null) return NotFound();

                if (subscription.UserId != userId.Value)
                    return Unauthorized();

                subscription.IsActive = false;
                subscription.EndDate = DateTime.Now;

                myDb.Update(subscription);
                myDb.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RedeemPoints()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var coupon = await _ecoPointsService.RedeemPoints(userId.Value);

            if (coupon != null)
            {
                TempData["SuccessMessage"] = $"تم استبدال النقاط بنجاح! كود الخصم: {coupon.Code} (خصم {coupon.DiscountPercentage}%) صالح حتى {coupon.ExpiryDate?.ToString("yyyy-MM-dd")}";
            }
            else
            {
                var user = await myDb.Users.FindAsync(userId.Value);
                var remainingPoints = _ecoPointsService.GetRemainingPointsForNextReward(user?.Points ?? 0);

                TempData["ErrorMessage"] = $"ليس لديك نقاط كافية للاستبدال. تحتاج {remainingPoints} نقطة إضافية";
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // أو حسب طريقة تسجيل الدخول عندك
            if (userId == null)
            {
                TempData["Error"] = "يجب تسجيل الدخول أولاً.";
                return RedirectToAction("Login");
            }

            var user = await myDb.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "المستخدم غير موجود.";
                return RedirectToAction("Profile"); // أو أي صفحة مناسبة
            }

            // تحقق من كلمة المرور الحالية
            if (!BCrypt.Net.BCrypt.Verify(CurrentPassword, user.Password)) // ⚠️ ملاحظة: يجب استخدام Hash في أنظمة حقيقية
            {
                TempData["Error"] = "كلمة المرور الحالية غير صحيحة.";
                return RedirectToAction("Profile");
            }

            // تحقق من تطابق الجديدة مع التأكيد
            if (NewPassword != ConfirmPassword)
            {
                TempData["Error"] = "كلمة المرور الجديدة وتأكيدها غير متطابقين.";
                return RedirectToAction("Profile");
            }

            // تحديث كلمة المرور
            user.Password = BCrypt.Net.BCrypt.HashPassword(NewPassword); ; // ⚠️ استخدم تشفير في أنظمة حقيقية
            myDb.Users.Update(user);
            myDb.SaveChanges();

            TempData["Success"] = "تم تغيير كلمة المرور بنجاح.";
            return RedirectToAction("Profile");
        }


        public async Task<IActionResult> RecyclingDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recyclingRequest = await myDb.RecyclingRequests
                .Include(r => r.User)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recyclingRequest == null)
            {
                return NotFound();
            }

            return View(recyclingRequest);
        }

    }
}