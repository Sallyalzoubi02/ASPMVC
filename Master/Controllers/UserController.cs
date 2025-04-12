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
        public IActionResult add2()
        {
            return View();
        }
        public IActionResult profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");



            var user = myDb.Users
       .Include(u => u.Orders)
           .ThenInclude(u => u.OrderItems)
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
        public IActionResult UpdateProfile(string firstName, string lastName, string email, string phone)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var user = myDb.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                // دمج الاسم الأول والأخير وحفظه في حقل Name
                user.Name = $"{firstName} {lastName}";
                user.Email = email;
                user.Phone = phone;
                myDb.SaveChanges();
                return RedirectToAction("profile");
            }

            return NotFound();
        }


        public static class NameHelper
        {
            // Splits "Ahmed Mohamed" into ("Ahmed", "Mohamed")
            public static (string FirstName, string LastName) SplitName(string fullName)
            {
                if (string.IsNullOrEmpty(fullName))
                    return ("", "");

                var parts = fullName.Split(' ', 2); // Split into max 2 parts
                return (parts[0], parts.Length > 1 ? parts[1] : "");
            }
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
