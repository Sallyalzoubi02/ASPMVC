using System.Diagnostics;
using Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Master.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext Db;

        public HomeController(ILogger<HomeController> logger , MyDbContext context )
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
                // تسجيل الدخول
                Response.Cookies.Append("UserId", user.Id.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7), // الكوكي تنتهي بعد أسبوع
                    HttpOnly = true, // تأمين الكوكي ضد الوصول عبر JavaScript
                    SameSite = SameSiteMode.Strict // تحديد سياسة SameSite
                });


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
