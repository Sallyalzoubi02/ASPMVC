using Master.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Text.Json; // لتحويل الجلسة إلى JSON
using Master.Extensions;

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
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null) // مستخدم مسجل
            {
                var cartItems = _db.Carts
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefault(c => c.UserId == userId)?
                    .CartItems.ToList() ?? new List<CartItem>();

                var appliedCouponJson = HttpContext.Session.GetString("AppliedCoupon");
                if (!string.IsNullOrEmpty(appliedCouponJson))
                {
                    var appliedCoupon = JsonSerializer.Deserialize<AppliedCoupon>(appliedCouponJson);
                    ViewBag.DiscountPercentage = appliedCoupon.DiscountPercentage;
                    ViewBag.DiscountAmount = appliedCoupon.DiscountAmount;
                }
                return View("Cart", cartItems);
            }
            else // زائر
            {
                var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart") ?? new List<TempCartItem>();
                var appliedCouponJson = HttpContext.Session.GetString("AppliedCoupon");
                if (!string.IsNullOrEmpty(appliedCouponJson))
                {
                    // Use the same deserialization approach as for logged-in users
                    var appliedCoupon = JsonSerializer.Deserialize<AppliedCoupon>(appliedCouponJson);
                    ViewBag.DiscountPercentage = appliedCoupon.DiscountPercentage;
                    ViewBag.DiscountAmount = appliedCoupon.DiscountAmount;
                }
                return View("Cart", tempCart);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity([FromBody] CartRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "يجب تسجيل الدخول" });

            try
            {
                var cartItem = _db.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefault(ci => ci.Cart.UserId == userId && ci.ProductId == request.ProductId);

                if (cartItem != null)
                {
                    cartItem.Quantity += request.Change;

                    if (cartItem.Quantity < 1)
                        cartItem.Quantity = 1;

                    _db.SaveChanges();

                    return Json(new
                    {
                        success = true,
                        newQuantity = cartItem.Quantity,
                        newPrice = cartItem.UnitPrice * cartItem.Quantity
                    });
                }

                return Json(new { success = false, message = "المنتج غير موجود في السلة" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveItem([FromBody] CartRequest request)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Json(new { success = false, message = "يجب تسجيل الدخول" });

            try
            {
                var cartItem = _db.CartItems
                    .Include(ci => ci.Cart)
                    .FirstOrDefault(ci => ci.Cart.UserId == userId && ci.ProductId == request.ProductId);

                if (cartItem != null)
                {
                    _db.CartItems.Remove(cartItem);
                    _db.SaveChanges();
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "المنتج غير موجود في السلة" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTempQuantity([FromBody] CartRequest request)
        {
            try
            {
                var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart") ?? new List<TempCartItem>();
                var item = tempCart.FirstOrDefault(i => i.ProductId == request.ProductId);

                if (item != null)
                {
                    item.Quantity += request.Change;
                    if (item.Quantity < 1) item.Quantity = 1;
                    HttpContext.Session.Set("TempCart", tempCart);
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "المنتج غير موجود" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveTempItem([FromBody] CartRequest request)
        {
            try
            {
                var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart") ?? new List<TempCartItem>();
                var item = tempCart.FirstOrDefault(i => i.ProductId == request.ProductId);

                if (item != null)
                {
                    tempCart.Remove(item);
                    HttpContext.Session.Set("TempCart", tempCart);
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "المنتج غير موجود" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public class CartRequest
        {
            public int ProductId { get; set; }
            public int Change { get; set; }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PrepareOrder()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                // إذا لم يكن مسجل دخول، احفظ السلة الحالية في الجلسة وأرسله لتسجيل الدخول
                HttpContext.Session.SetString("fromPayment", "true");
                return RedirectToAction("Sign", "Home");
            }

            // إذا كان مسجل دخول، تحقق من وجود عناصر في السلة
            var cartItems = _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId)?
                .CartItems.ToList();


            if (cartItems == null || !cartItems.Any())
            {
                ViewBag.Message = "Cart is empty";
                return RedirectToAction("Shop");
            }


            // توجيه إلى صفحة الدفع
            return RedirectToAction("Payment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder([FromBody] ConfirmOrderViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); // تابع بيجيب الآيدي من السيشن أو الهوية

            if (userId == null)
                return Json(new { success = false, message = "يجب تسجيل الدخول" });

            var cartItems = _db.CartItems
                .Where(c => c.Cart.UserId == userId)
                .Include(c => c.Product)
                .ToList();

            if (!cartItems.Any())
                return Json(new { success = false, message = "السلة فارغة" });

            var totalAmount = cartItems.Sum(c => c.TotalPrice ?? (c.UnitPrice * c.Quantity));


            var paymentStatus = model.PaymentMethod == "credit" ? "تم الدفع" : "بانتظار الدفع";

            // 1- Payment
            var payment = new Payment
            {
                UserId = userId,
                Amount = totalAmount,
                PaymentMethod = model.PaymentMethod,
                PaymentType = "Order",
                Status = paymentStatus,
                CreatedAt = DateTime.Now
            };

            _db.Payments.Add(payment);
            await _db.SaveChangesAsync();

            // 2- Order
            var order = new Order
            {
                UserId = userId,
                PaymentId = payment.Id,
                OrderStatus = "جاري المعالجة",
                DeliveryAddress = model.Address1 + " " + model.Address2,
                DeliveryTime = DateTime.Now.AddHours(1),
                TotalAmount = totalAmount,
                CreatedAt = DateTime.Now,
                Address1 = model.Address1,
                Address2 = model.Address2,
                Phone = int.Parse(model.Phone)
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // 3- OrderItems
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.TotalPrice ?? (item.UnitPrice * item.Quantity)
                };
                _db.OrderItems.Add(orderItem);
            }

            // 4- حذف cart items
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return RedirectToAction("profile","User");
        }


        private void TransferTempCartToUserCart(int userId)
        {
            var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart");
            if (tempCart == null || !tempCart.Any()) return;

            var cart = _db.Carts
                .Include(c => c.CartItems)
                .FirstOrDefault(c => c.UserId == userId) ?? new Cart { UserId = userId };

            if (cart.Id == 0)
            {
                _db.Carts.Add(cart);
                _db.SaveChanges();
            }

            foreach (var tempItem in tempCart)
            {
                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == tempItem.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += tempItem.Quantity;
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = tempItem.ProductId,
                        Quantity = tempItem.Quantity,
                        UnitPrice = tempItem.Price,
                        CreatedAt = DateTime.Now
                    });
                }
            }

            _db.SaveChanges();
            HttpContext.Session.Remove("TempCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApplyCoupon(string couponCode)
        {
            try
            {
                if (string.IsNullOrEmpty(couponCode))
                {
                    return Json(new { success = false, message = "الرجاء إدخال كود الخصم" });
                }

                // البحث عن الكوبون في قاعدة البيانات
                var coupon = _db.Coupons.FirstOrDefault(c => c.Code == couponCode &&
                                                           (c.ExpiryDate == null || c.ExpiryDate >= DateTime.Now) &&
                                                           c.IsActive);

                if (coupon == null)
                {
                    return Json(new { success = false, message = "كود الخصم غير صالح أو منتهي الصلاحية" });
                }

                // حساب المجموع الجزئي للسلة
                var subtotal = CalculateCartSubtotal();

                // حساب الخصم والمجموع النهائي
                var discountAmount = subtotal * (coupon.DiscountPercentage / 100);
                var total = subtotal - discountAmount ;

                // تخزين بيانات الخصم في الجلسة
                HttpContext.Session.SetString("AppliedCoupon", JsonSerializer.Serialize(new
                {
                    Code = coupon.Code,
                    DiscountPercentage = coupon.DiscountPercentage,
                    DiscountAmount = discountAmount
                }));

                return Json(new
                {
                    success = true,
                    subtotal = subtotal.ToString("0.00"),
                    discountAmount = discountAmount.ToString("0.00"),
                    total = total.ToString("0.00"),
                    message = $"تم تطبيق خصم {coupon.DiscountPercentage}% بنجاح"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء معالجة الكوبون" });
            }
        }

        private decimal CalculateCartSubtotal()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            decimal subtotal = 0;

            if (userId != null)
            {
                // حساب المجموع للسلة الدائمة للمستخدم المسجل
                subtotal = _db.CartItems
                    .Include(ci => ci.Cart)
                    .Where(ci => ci.Cart.UserId == userId)
                    .Sum(ci => ci.UnitPrice * ci.Quantity);
            }
            else
            {
                // حساب المجموع للسلة المؤقتة للزائر
                var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart") ?? new List<TempCartItem>();
                subtotal = tempCart.Sum(item => item.Price * item.Quantity);
            }

            return subtotal;
        }

        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
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
            }

            else {
                var product = _db.Products.Find(productId);
                var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart") ?? new List<TempCartItem>();

                var existingItem = tempCart.FirstOrDefault(item => item.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    tempCart.Add(new TempCartItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Price = product.Price,
                        Quantity = quantity,
                        ImageUrl = product.ImageUrl
                    });
                }
                HttpContext.Session.Set("TempCart", tempCart);
                Console.Write(HttpContext.Session.Get("TempCart"));
            }

            return RedirectToAction("Shop", "Shop"); // أو عرض رسالة نجاح



        }

        //[HttpPost]
        //public IActionResult AddToTempCart(int productId, int quantity = 1)
        //{
        //    var product = _db.Products.Find(productId);
        //    if (product == null) return NotFound();

        //    // جلب السلة الحالية من الجلسة
        //    var tempCart = HttpContext.Session.Get<List<TempCartItem>>("TempCart") ?? new List<TempCartItem>();

        //    // التحقق إذا كان المنتج موجودًا مسبقًا
        //    var existingItem = tempCart.FirstOrDefault(item => item.ProductId == productId);
        //    if (existingItem != null)
        //    {
        //        existingItem.Quantity += quantity;
        //    }
        //    else
        //    {
        //        tempCart.Add(new TempCartItem
        //        {
        //            ProductId = product.Id,
        //            ProductName = product.Name,
        //            Price = product.Price,
        //            Quantity = quantity,
        //            ImageUrl = product.ImageUrl
        //        });
        //    }

        //    // حفظ السلة في الجلسة
        //    HttpContext.Session.Set("TempCart", tempCart);

        //    return RedirectToAction("Shop");
        //}
        public IActionResult Shop(string category, string company, string priceSort, int? productType)
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

        public IActionResult Payment()
        {
            return View();
        }
    }

}