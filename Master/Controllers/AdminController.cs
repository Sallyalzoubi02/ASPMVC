using Master.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using static System.Net.Mime.MediaTypeNames;

namespace Master.Controllers
{

    public class AdminController : Controller
    {
        private readonly MyDBContext _dbContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AdminController(MyDBContext myDBContext, IWebHostEnvironment hostingEnvironment)
        {

            _dbContext = myDBContext;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: AdminController
        public ActionResult Index()
        {
            // إحصائيات العملاء
            var clientsCount = _dbContext.Users.Count();

            // إحصائيات المبيعات
            var salesCount = _dbContext.Orders.Count();

            // إحصائيات الإيرادات
            var revenue = _dbContext.Payments.Where(p => p.Status == "تم الدفع")?.Sum(p => p.Amount) ?? 0;

            // حساب النمو (نسبة الزيادة في المبيعات هذا الشهر مقارنة بالشهر الماضي)
            var currentMonthSales = _dbContext.Orders
                .Count(o => o.CreatedAt.HasValue &&
                           o.CreatedAt.Value.Month == DateTime.Now.Month &&
                           o.CreatedAt.Value.Year == DateTime.Now.Year);

            var lastMonthSales = _dbContext.Orders
                .Count(o => o.CreatedAt.HasValue &&
                           o.CreatedAt.Value.Month == DateTime.Now.AddMonths(-1).Month &&
                           o.CreatedAt.Value.Year == DateTime.Now.AddMonths(-1).Year);

            var growthPercentage = lastMonthSales > 0 ?
                ((currentMonthSales - lastMonthSales) * 100 / lastMonthSales) :
                (currentMonthSales > 0 ? 100 : 0);

            // بيانات المبيعات الشهرية للرسم البياني
            var monthlySalesData = _dbContext.Orders
                .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value.Year == DateTime.Now.Year)
                .AsEnumerable() // للانتقال إلى العمليات على مستوى الذاكرة
                .GroupBy(o => o.CreatedAt.Value.Month)
                .Select(g => new {
                    Month = g.Key,
                    Sales = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            // ملء البيانات لجميع الأشهر (حتى لو لم يكن هناك مبيعات)
            var allMonthsData = Enumerable.Range(1, 12)
                .Select(month => new {
                    Month = month,
                    Sales = monthlySalesData.FirstOrDefault(m => m.Month == month)?.Sales ?? 0
                })
                .ToList();

            // أفضل المنتجات مبيعاً
            var topProducts = _dbContext.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.Product)
                .Select(g => new {
                    Product = g.Key,
                    SalesCount = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.Product.Price)
                })
                .OrderByDescending(x => x.SalesCount)
                .Take(5)
                .ToList();

            // أحدث الطلبات
            var latestOrders = _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(3)
                .ToList();

            // تمرير البيانات إلى العرض
            ViewBag.ClientsCount = clientsCount;
            ViewBag.SalesCount = salesCount;
            ViewBag.Revenue = revenue;
            ViewBag.GrowthPercentage = growthPercentage;
            ViewBag.MonthlySales = allMonthsData;
            ViewBag.TopProducts = topProducts;
            ViewBag.LatestOrders = latestOrders;

            // حساب الرسائل غير المقروءة وإضافتها لـ ViewBag
            // استخدم .Result (لكن احذر قد يسبب Deadlocks في بعض الحالات)
            ViewBag.UnreadMessagesCount = _dbContext.Contacts.CountAsync(c => !c.IsRead).Result;
            return View();
        }

        public ActionResult logout()
        {
            return RedirectToAction("Index", "Home");
        }
        //----------------------------category-------------------------------

        public ActionResult Category()
        {
            var category = _dbContext.Categories.Include(c => c.Products) 
        .ToList();
            if (category.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(category);
            }

            return View(category);
        }


        // GET: AdminController/CreateCategory
        public ActionResult CreateCategory()
        {

            return View();
        }

        // POST: AdminController/CreateCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
                TempData["Success"] = "تمت إضافة الصنف بنجاح.";
                return RedirectToAction("Category");
            }

            return View(category);
        }

        // GET: AdminController/EditCategory
        [HttpPost]
        public ActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(category);
                _dbContext.SaveChanges();
                return RedirectToAction("Category");
            }

            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCategory(int id)
        {
            var category = _dbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            if (category.Products.Any())
            {
                TempData["DeleteError"] = "لا يمكنك حذف هذا الصنف لأنه يحتوي على منتجات.";
                return RedirectToAction("Category");
            }

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();

            TempData["Success"] = "تم حذف الصنف بنجاح.";
            return RedirectToAction("Category");
        }





        //---------------------------products---------------------------------

        public ActionResult Products()
        {
            var products = _dbContext.Products.Include(p => p.Category);
            if (products.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(products);
            }

            return View(products);
        }
        // GET: Product/EditProduct/id
        public async Task<IActionResult> EditProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = _dbContext.Categories
        .Select(c => new SelectListItem
        {
            Value = c.Id.ToString(),
            Text = c.CategoryName
        }).ToList();
            return View(product);
        }

        // POST: Product/EditProduct/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, [Bind("Id,Name,Description,Price,Stock,CompanyName,ImageUrl,Type,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.CreatedAt = DateTime.Now;
                    _dbContext.Update(product);
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Products");
            }
            ViewBag.Categories = await _dbContext.Categories.ToListAsync();
            return View(product);
        }

        // GET: Product/DetailsProduct/id
        public async Task<IActionResult> DetailsProduct(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //Post: Product/DeleteProduct/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _dbContext.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // حذف الصورة المرتبطة إذا وجدت
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, product.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم حذف المنتج بنجاح";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء محاولة الحذف: " + ex.Message;
            }

            return RedirectToAction(nameof(Products));
        }

        private bool ProductExists(int id)
        {
            return _dbContext.Products.Any(e => e.Id == id);
        }

        // GET: Product/CreateProduct
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _dbContext.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View();
        }

        // POST: Product/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // معالجة رفع الصورة
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "image");

                    // إنشاء المجلد إذا لم يكن موجوداً
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "image/" + uniqueFileName;
                }

                product.CreatedAt = DateTime.Now;
                _dbContext.Add(product);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _dbContext.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.CategoryName
                }).ToList();

            return View(product);
        }






        //-----------------------------Users---------------------------

        public ActionResult Users()
        {
            var users = _dbContext.Users.Where(x => x.UserType != "admin").ToList();
            if (users.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(users);
            }

            return View(users);
        }


        //-----------------------------Payment---------------------------

        public ActionResult Payment()
        {
            var payments = _dbContext.Payments.Include(p => p.User).ToList();
            if (payments.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(payments);
            }

            return View(payments);
        }




        //----------------------------Orders-----------------------------

        public ActionResult Orders()
        {
            var orders = _dbContext.Orders
                        .Include(u => u.User)
                        .Include(u => u.Payment)
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product) 
                        .ToList();
            if (orders.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(orders);
            }

            return View(orders);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string PaymentStatus)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
            var pay = _dbContext.Payments.FirstOrDefault(p => p.Id == order.PaymentId);
            if (pay != null)
            {
                pay.Status = PaymentStatus;
                _dbContext.Payments.Update(pay);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Orders"); // أو اسم صفحة عرض الطلبات
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, string OrderStatus)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                order.OrderStatus = OrderStatus;
                _dbContext.Orders.Update(order);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Orders"); // أو اسم صفحة عرض الطلبات
        }

        //---------------------------Recycling---------------------------

        public ActionResult Recycling()
        {
            var recycling = _dbContext.RecyclingRequests.Include(u => u.User).Include(p => p.Payment).ToList();
            return View(recycling);
        }
        [HttpPost]
        public IActionResult UpdateDeliveryStatus(int id, bool delstatus)
        {
            var recyclingRequest = _dbContext.RecyclingRequests.FirstOrDefault(o => o.Id == id);
            if (recyclingRequest != null)
            {
                recyclingRequest.DeliveryStatus = delstatus;
                _dbContext.RecyclingRequests.Update(recyclingRequest);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Recycling"); // أو اسم صفحة عرض الطلبات
        }

        //---------------------------VolunteerApplication-----------------


        public ActionResult VolunteerApplication()
        {
            var vol = _dbContext.VolunteerApplications
                        .ToList();
            if (vol.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(vol);
            }

            return View(vol);
        }

        [HttpPost]
        public IActionResult UpdateVolStatus(int id, string volstatus)
        {
            var vol = _dbContext.VolunteerApplications.FirstOrDefault(o => o.Id == id);
            if (vol != null)
            {
                vol.Status = volstatus;
                _dbContext.VolunteerApplications.Update(vol);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("VolunteerApplication"); // أو اسم صفحة عرض الطلبات
        }

        //---------------------------EmploymentApplication-----------------------------
        public ActionResult EmploymentApplication()
        {
            var vol = _dbContext.EmploymentApplications
                        .ToList();
            if (vol.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(vol);
            }

            return View(vol);
        }

        [HttpPost]
        public IActionResult UpdateEmpStatus(int id, string volstatus)
        {
            var vol = _dbContext.EmploymentApplications.FirstOrDefault(o => o.Id == id);
            if (vol != null)
            {
                vol.Status = volstatus;
                _dbContext.EmploymentApplications.Update(vol);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("EmploymentApplication"); // أو اسم صفحة عرض الطلبات
        }

        

        [HttpPost]
        public async Task<IActionResult> SendReply(int id,string email, string status,string replyMessage,[FromServices] EmailService emailService)
        {
            try
            {
                // تحديث حالة الطلب في قاعدة البيانات
                var application = await _dbContext.EmploymentApplications.FindAsync(id);
                if (application == null)
                {
                    return Json(new { success = false, message = "لم يتم العثور على الطلب" });
                }

                application.Status = status;
                await _dbContext.SaveChangesAsync();

                // إرسال البريد الإلكتروني
                string subject = $"حالة طلب التوظيف - {status}";
                string emailBody = $@"
            <h2>مرحباً {application.FullName},</h2>
            <p>شكراً لتقديمك طلب توظيف معنا.</p>
            <p>حالة طلبك: <strong>{status}</strong></p>
            <p>رسالتنا لك:</p>
            <div style='background-color:#f8f9fa; padding:15px; border-radius:5px;'>
                {replyMessage}
            </div>
            <p>مع تحياتنا،<br/>فريق التوظيف</p>";


                await emailService.SendEmailAsync(email, subject, emailBody);

                TempData["ToastrMessage"] = "تم إرسال الرد بنجاح";
                TempData["ToastrType"] = "success";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                TempData["ToastrMessage"] = "حدث خطأ أثناء الإرسال";
                TempData["ToastrType"] = "error";
                return Json(new { success = false });
            }
        }


        //------------------------------------subscription---------------------------------------

        public ActionResult subscription()
        {
            var sub = _dbContext.Subscriptions.Include(x=>x.User)
                        .ToList();
            if (sub.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(sub);
            }

            return View(sub);
        }



        //--------------------------------Vouchers-------------------------------------------------
        public ActionResult Voucher()
        {
            var now = DateTime.Now; // التاريخ والوقت الحالي
            var vouchers = _dbContext.Coupons.ToList();

            if (vouchers.IsNullOrEmpty())
            {
                ViewBag.IsNull = true;
                return View(vouchers);
            }

            // التحقق من كل كوبون وتحديث حالته إذا لزم الأمر
            foreach (var voucher in vouchers)
            {
                if (voucher.ExpiryDate < now && voucher.IsActive)
                {
                    voucher.IsActive = false;
                    // يمكنك إضافة حفظ التغييرات هنا إذا أردت تحديث قاعدة البيانات
                    // _dbContext.SaveChanges();
                }
            }

            // إذا كنت تريد حفظ جميع التغييرات مرة واحدة بعد الحلقة
            _dbContext.SaveChanges();

            return View(vouchers);
        }


        //--------------------------------Messages-------------------------------------------------

        // لوحة إدارة الرسائل (للمسؤول)
        public async Task<IActionResult> Messages()
        {
            var messages = await _dbContext.Contacts
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(messages);
        }

        // عرض رسالة معينة
        public async Task<IActionResult> MessageDetails(int id)
        {
            var message = await _dbContext.Contacts.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            // وضع علامة مقروءة عند فتح الرسالة
            if (!message.IsRead)
            {
                message.IsRead = true;
                _dbContext.Update(message);
                await _dbContext.SaveChangesAsync();
            }

            return View(message);
        }


        // إرسال رد
        [HttpPost]
        public async Task<IActionResult> Reply(int id, string replyMessage, [FromServices] EmailService emailService)
        {
            var message = await _dbContext.Contacts.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            message.ReplyMessage = replyMessage;
            message.ReplyDate = DateTime.Now;
            _dbContext.Update(message);
            await _dbContext.SaveChangesAsync();


            // إرسال البريد الإلكتروني
            string subject = $"نقدر تغذيتك الراجعه و مشاركة رأيك حول الخدمات";
            string emailBody = $@"
            <h2>مرحباً {message.FirstName} {message.LastName},</h2>
            <p>نقدر تغذيتك الراجعه و مشاركة رأيك حول الخدمات</p>
            <div style='background-color:#f8f9fa; padding:15px; border-radius:5px;'>
                <p>{message.ReplyMessage}</p>
            </div>
            <p>مع تحياتنا،<br/>فريق أثر</p>";


            await emailService.SendEmailAsync(message.Email, subject, emailBody);

            
            return RedirectToAction("MessageDetails", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadMessagesCount()
        {
            var count = await _dbContext.Contacts.CountAsync(c => !c.IsRead);
            return Json(count);
        }


    }
}
