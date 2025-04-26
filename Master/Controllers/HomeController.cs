using System.Diagnostics;
using Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json; // لتحويل الجلسة إلى JSON
using Master.Extensions;

namespace Master.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDBContext Db;

        public HomeController(ILogger<HomeController> logger , MyDBContext context )
        {
            _logger = logger;
            Db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

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


        public IActionResult About()
        {
            return View();
        }

       

        public IActionResult Sign(string formType)
        {
            ViewBag.FormType = formType ?? "login";
            return View();
        }
        [HttpPost]
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
                


                HttpContext.Session.SetString("logged", "true");
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction("index", "Home");
            }

            TempData["ErrorMessage"] = "البريد الإلكتروني أو كلمة المرور غير صحيحة";
            return RedirectToAction("Sign", new { formType = "login" });
        }


        [HttpPost]
        public IActionResult Register(string Name, string email, string phone, string city,
                              string location,DateOnly Bdate, string accont, string gender, string password, string? companyName)
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

            Db.Users.Add(user);
            Db.SaveChanges();

            // تسجيل المستخدم تلقائي
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);

            if (accont == "company" && !string.IsNullOrWhiteSpace(companyName))
            {
                var company = new Company
                {
                    CompanyName = companyName,
                    OwnerId = user.Id // الربط مع المستخدم كـ مالك
                };
                Db.Companies.Add(company);
                Db.SaveChanges();
            }

            return RedirectToAction("Profile", "User");
        }


        public IActionResult forgetPassword()
        {
            return View();
        }

        public IActionResult recycling()
        {
            return View();
        }

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
