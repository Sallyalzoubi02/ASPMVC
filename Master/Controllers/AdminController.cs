using Master.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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
        // GET: AdminController/Details/5
        

        // GET: AdminController/Create
        public ActionResult CreateCategory()
        {

            return View();
        }

        // POST: AdminController/Create
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
        // GET: Product/EditProduct/5
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
            var users = _dbContext.Users.ToList();
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

    }
}
