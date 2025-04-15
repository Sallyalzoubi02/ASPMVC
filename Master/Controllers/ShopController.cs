using Master.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Master.Controllers
{
    public class ShopController : Controller
    {
        private readonly MyDbContext _db;

        public ShopController(MyDbContext db)
        {
            _db = db;
        }

        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // غير مسجل
            }

            // البحث عن Cart أو إنشائه إذا لم يوجد
            var cart = _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId) ?? new Cart { UserId = userId.Value };

            if (cart.Id == 0) // إذا كان Cart جديدًا
            {
                _db.Carts.Add(cart);
                _db.SaveChanges(); // لحفظ الـ Cart أولًا والحصول على الـ Id
            }

            // إضافة المنتج إلى الـ CartItem
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity; // زيادة الكمية إذا المنتج موجود مسبقًا
            }
            else
            {
                var product = _db.Products.Find(productId);
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    CreatedAt = DateTime.Now
                });
            }

            _db.SaveChanges();
            return RedirectToAction("Shop", "Shop"); // أو عرض رسالة نجاح
        }
        public IActionResult Shop(string category, string company, string priceSort , int? productType)
        {
            var query = _db.Products
                .Include(p => p.Category)
                .AsQueryable();

            // Apply filters if they exist
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.CategoryName == category);
            }

            if (!string.IsNullOrEmpty(company))
            {
                query = query.Where(p => p.CompanyName == company);
            }

            // Apply type filter if specified
            if (productType.HasValue)
            {
                query = query.Where(p => p.Type == (productType.Value == 1));
            }

            // Apply price sorting
            if (!string.IsNullOrEmpty(priceSort))
            {
                query = priceSort == "max"
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price);
            }

            ViewBag.Companies = _db.Products
                .Select(p => p.CompanyName)
                .Distinct()
                .ToList();
            ViewBag.Categories = _db.Categories
                .Select(c => c.CategoryName)
                .Distinct()
                .ToList();

            return View(query.ToList());
        }


        public IActionResult ProductDetails(int id)
        {
            var product = _db.Products
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}