using System.Net;
using System.Security.Claims;
using Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Master.Controllers
{
    public class UserController : Controller
    {
        private readonly MyDbContext myDb;
        //public int userId;
        public UserController(MyDbContext myDbContext)
        {
            myDb = myDbContext;
            //userId = int.Parse(Request.Cookies["UserId"]);
        }
        public IActionResult add()
        {
            return View();
        }
        
        public IActionResult profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");



            var user = myDb.Users
       .Include(u => u.Orders)
           .ThenInclude(u => u.OrderItems)
                               .ThenInclude(oi => oi.Product) // تأكد من تحميل المنتج

       .Include(u => u.RecyclingRequests)
       .Include(u => u.Companies)
       .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            // تحضير نموذج العرض
            var model = new UserProfileViewModel
            {
                User = user,
                Orders = user.Orders.ToList(),
                RecyclingRequests = user.RecyclingRequests.ToList(),
                IsOwner = user.UserType == "owner",
                Companies = user.Companies.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateProfile(string Name, DateOnly birth_date, string email, string phone)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = myDb.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                // دمج الاسم الأول والأخير وحفظه في حقل Name
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

            var order = myDb.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // تأكد من تحميل المنتج
                .FirstOrDefault(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound();

            // تسجيل البيانات للتحقق منها
            foreach (var item in order.OrderItems)
            {
                Console.WriteLine($"OrderItem ID: {item.Id}, Product: {(item.Product != null ? item.Product.Name : "NULL")}");
            }

            return View(order);
        }


        public IActionResult Logout()
        {
            // حذف السيشن
            HttpContext.Session.Remove("logged");
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("UserName");

            // حذف الكوكي إذا كان موجودًا
            Response.Cookies.Delete("UserId");

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Subscribe() {
            return View();
        }
    }
}
