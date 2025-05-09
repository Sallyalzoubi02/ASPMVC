using System;
using System.Linq;
using System.Security.Claims;
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

        //[HttpPost]
        //public async Task<IActionResult> AddRecyclingRequest([FromForm] RecyclingRequestVm requestDto)
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null)
        //        return Json(new { success = false, redirect = Url.Action("Login", "Auth") });

        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values.SelectMany(v => v.Errors)
        //                            .Select(e => e.ErrorMessage)
        //                            .ToList();
        //        return Json(new { success = false, message = "بيانات غير صالحة", errors });
        //    }

        //    try
        //    {
        //        var requestedDateTime = DateTime.Parse($"{requestDto.RequestedDate} {requestDto.RequestedTime}");

        //        if (requestedDateTime <= DateTime.Now)
        //            return Json(new { success = false, message = "لا يمكن اختيار تاريخ في الماضي" });

        //        // باقي كود معالجة البيانات...

        //        await myDb.SaveChangesAsync();

        //        return Json(new
        //        {
        //            success = true,
        //            redirect = Url.Action("Profile"),
        //            message = "تم إضافة الطلب بنجاح"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = "حدث خطأ أثناء حفظ البيانات",
        //            error = ex.Message
        //        });
        //    }
        //}

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

        private decimal CalculateExpressFee(string city)
        {
            return city switch
            {
                "Amman" => 5.00m,
                "Irbid" => 7.00m,
                _ => 10.00m
            };
        }

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
                .FirstOrDefault(u => u.Id == userId);

            if (user == null) return NotFound();

            var model = new UserProfileViewModel
            {
                User = user,
                Orders = user.Orders.ToList(),
                RecyclingRequests = user.RecyclingRequests.ToList(),
                IsOwner = user.UserType == "owner",
                Companies = user.Companies.ToList(),
                // إضافة اشتراكات المستخدم
                ActiveSubscriptions = myDb.Subscriptions
                    .Where(s => s.UserId == userId && s.IsActive)
                    .ToList()
            };

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
            HttpContext.Session.Remove("UserName");
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
    }
}