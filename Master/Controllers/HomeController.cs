using System.Diagnostics;
using Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Master.Extensions;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;


namespace Master.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDBContext Db;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger , MyDBContext context , EmailService emailService)
        {
            _logger = logger;
            Db = context;
            _emailService = emailService;
        }


        //---------------------------------------------Home-------------------------------
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null) {
                var user = Db.Users
                .Include(u => u.Companies)
                .FirstOrDefault(u => u.Id == userId);

                if (user == null) return NotFound();

                // التحقق إذا فيه شركة وبياناتها ناقصة (هنا مثال بسيط)
                bool incompleteCompanyInfo = !(user.Companies.Any(u => u.OwnerId == user.Id));
                HttpContext.Session.SetString("IncompleteCompany", incompleteCompanyInfo ? "true" : "false");

            }

            
            return View();
        }
        //---------------------------------------------Contact us-------------------------------

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(Contact model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Db.Contacts.Add(model);
                    await Db.SaveChangesAsync();

                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        // إذا كان الطلب AJAX
                        return Json(new { success = true });
                    }

                    return RedirectToAction(nameof(Contact));
                }
                catch (Exception ex)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    {
                        return Json(new { success = false, error = ex.Message });
                    }

                    ModelState.AddModelError("", "حدث خطأ أثناء محاولة إرسال الرسالة: " + ex.Message);
                }
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
                return Json(new { success = false, errors = errors });
            }

            return View(model);
        }

        //---------------------------------------------About-------------------------------

        public IActionResult About()
        {
            ViewBag.Users = Db.Users.Count();
            ViewBag.Volnteer = Db.VolunteerApplications.Where(v => v.Status == "مقبول").Count();
            ViewBag.Employee = Db.EmploymentApplications.Where(v => v.Status == "مقبول").Count();
            return View();
        }


        //---------------------------------------------Sign-------------------------------

        public IActionResult Sign(string formType)
        {
            ViewBag.FormType = formType ?? "login";
            return View();
        }
    
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // التأكد من أن المدخلات غير فارغة
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "البريد الإلكتروني وكلمة المرور مطلوبان";
                return RedirectToAction("Sign", new { formType = "login" });
            }

            if (email == "admin@athar.com" && password == "admin")
            {
                HttpContext.Session.SetString("logged", "true");

                return RedirectToAction("Index", "Admin");
            }
            // محاولة العثور على المستخدم
            var user = Db.Users.SingleOrDefault(u => u.Email == email);

            // التحقق إذا كان المستخدم موجودًا
            if (user == null)
            {
                TempData["ErrorMessage"] = "البريد الإلكتروني غير موجود";
                return RedirectToAction("Sign", new { formType = "login" });
            }

            var password1 = password;
            var Virify = user.Password;

            if (BCrypt.Net.BCrypt.Verify(password1, Virify))
            {
                if (HttpContext.Session.GetString("fromPayment") == "true")
                {
                    var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart");
                    if (tempCart != null && tempCart.Any())
                    {
                        // 2. البحث عن سلة المستخدم الحالية
                        var userCart = Db.Carts
                            .Include(c => c.CartItems)
                            .FirstOrDefault(c => c.UserId == user.Id);

                        // 3. إذا لم يكن لديه سلة، ننشئ واحدة جديدة
                        if (userCart == null)
                        {
                            userCart = new Cart
                            {
                                UserId = user.Id
                            };
                            Db.Carts.Add(userCart);
                            Db.SaveChanges();
                        }
                        foreach (var tempItem in tempCart)
                        {
                            var existingItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == tempItem.ProductId);

                            if (existingItem != null)
                            {
                                // إذا كان المنتج موجودًا بالفعل، نزيد الكمية
                                existingItem.Quantity += tempItem.Quantity;
                                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                            }
                            else
                            {
                                // إذا كان المنتج غير موجود، نضيفه جديدًا
                                userCart.CartItems.Add(new CartItem
                                {
                                    ProductId = tempItem.ProductId,
                                    Quantity = tempItem.Quantity,
                                    UnitPrice = tempItem.Price,
                                    TotalPrice = tempItem.Price * tempItem.Quantity,
                                    CreatedAt = DateTime.Now
                                });
                            }
                        }

                        Db.SaveChanges();

                        // 5. مسح السلة المؤقتة من السيشن
                        HttpContext.Session.Remove("TempCart");
                        HttpContext.Session.Remove("fromPayment");
                        }
                    }

                // 1. إعداد الـ Claims للمصادقة
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.UserType) // لازم تكون Admin مثل قاعدة البيانات
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // 2. تسجيل الدخول باستخدام الكوكيز
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("logged", "true");
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserType", user.UserType);

                return RedirectToAction("index", "Home");
            }

            TempData["ErrorMessage"] = "البريد الإلكتروني أو كلمة المرور غير صحيحة";
            return RedirectToAction("Sign", new { formType = "login" });
        }


        public IActionResult Register(string Name, string email, string phone, string city,
                      string location, DateOnly Bdate, string accont, string gender,
                      string password, string? companyName)
        {
            var existingUser = Db.Users.SingleOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                TempData["ErrorMessage"] = "البريد الإلكتروني مستخدم مسبقًا";
                return RedirectToAction("Sign", new { formType = "register" });
            }

            var user = new User
            {
                Name = Name,
                Email = email,
                Phone = phone,
                City = city,
                BirthDate = Bdate,
                Location = location,
                UserType = accont,
                Gender = gender,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Points = 0
            };

            // 🔹 أولًا: أضف المستخدم واحفظه للحصول على Id صالح
            Db.Users.Add(user);
            Db.SaveChanges();

            // 🔹 ثانيًا: إذا التسجيل من صفحة الدفع، اربط السلة المؤقتة بالمستخدم الجديد
            if (HttpContext.Session.GetString("fromPayment") == "true")
            {
                var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart");
                if (tempCart != null && tempCart.Any())
                {
                    var userCart = Db.Carts
                        .Include(c => c.CartItems)
                        .FirstOrDefault(c => c.UserId == user.Id);

                    if (userCart == null)
                    {
                        userCart = new Cart
                        {
                            UserId = user.Id
                        };
                        Db.Carts.Add(userCart);
                        Db.SaveChanges();
                    }

                    foreach (var tempItem in tempCart)
                    {
                        var existingItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == tempItem.ProductId);

                        if (existingItem != null)
                        {
                            existingItem.Quantity += tempItem.Quantity;
                            existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                        }
                        else
                        {
                            userCart.CartItems.Add(new CartItem
                            {
                                ProductId = tempItem.ProductId,
                                Quantity = tempItem.Quantity,
                                UnitPrice = tempItem.Price,
                                TotalPrice = tempItem.Price * tempItem.Quantity,
                                CreatedAt = DateTime.Now
                            });
                        }
                    }

                    Db.SaveChanges();

                    // حذف السلة المؤقتة من السيشن
                    HttpContext.Session.Remove("TempCart");
                    HttpContext.Session.Remove("fromPayment");
                }
            }

            // 🔹 ثالثًا: تسجيل الدخول تلقائيًا
            //HttpContext.Session.SetInt32("UserId", user.Id);
            //HttpContext.Session.SetString("UserName", user.Name);
            //HttpContext.Session.SetString("logged", "true");

            // 🔹 رابعًا: إذا كان الحساب شركة، أضف بيانات الشركة
            //if (accont == "company" && !string.IsNullOrWhiteSpace(companyName))
            //{
            //    var company = new Company
            //    {
            //        CompanyName = companyName,
            //        OwnerId = user.Id // ربط الشركة بالمستخدم
            //    };
            //    Db.Companies.Add(company);
            //    Db.SaveChanges();
            //}

            // 🔹 وأخيرًا: إعادة التوجيه لصفحة البروفايل
            return RedirectToAction("Home", "Sign");
        }

        //-------------------------------forgetPassword------------------------------------------------------
        public IActionResult forgetPassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            // التحقق من وجود البريد في قاعدة البيانات
            var user = Db.Users.SingleOrDefault(u => u.Email == email);
            return Json(new { exists = user != null });
        }

        [HttpPost]
        public async Task<IActionResult> SendOTP([FromBody] OTPRequest request)
        {
            try
            {
                await _emailService.SendEmailAsync(request.Email,
                    "كود التحقق لإعادة تعيين كلمة المرور",
                    $"كود التحقق الخاص بك هو: {request.OTP}");

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ أثناء إرسال البريد الإلكتروني");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = Db.Users.SingleOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return NotFound("المستخدم غير موجود");
            }

            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                Db.Users.Update(user);
                await Db.SaveChangesAsync();

                return Ok("تم تحديث كلمة المرور بنجاح");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "حدث خطأ أثناء تحديث كلمة المرور: " + ex.Message);
            }
        }


        public class OTPRequest
        {
            public string Email { get; set; }
            public string OTP { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string Email { get; set; }
            public string NewPassword { get; set; }
        }


        //---------------------------------------------recycling-------------------------------
        public IActionResult recycling()
        {
            return View();
        }
        //---------------------------------------------Join us-------------------------------

        public IActionResult JoinUs()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
