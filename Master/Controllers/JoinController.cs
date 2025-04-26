using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Master.Models;
using System.Diagnostics;

namespace Master.Controllers
{
    public class JoinController : Controller
    {
        private readonly MyDBContext _context;

        public JoinController(MyDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEmployment([FromBody] EmploymentApplication model)
        {
            
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "الرجاء ملء جميع الحقول المطلوبة" });
            }

            try
            {
                var application = new EmploymentApplication
                {
                    FullName = model.FullName,
                    NationalId = model.NationalId,
                    Phone = model.Phone,
                    Email = model.Email,
                    City = model.City,
                    District = model.District,
                    ExperienceYears = model.ExperienceYears,
                    Position = model.Position,
                    MotivationText = model.MotivationText,
                    ApplicationDate = DateTime.Now,
                    Status = "Pending"
                };

                _context.EmploymentApplications.Add(application);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "تم استلام طلب التوظيف بنجاح، شكراً لك!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء إرسال الطلب: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitVolunteer([FromBody] VolunteerApplicationDto dto)
        {
            if (dto == null) return BadRequest("No data received");

            var application = new VolunteerApplication
            {
                FullName = dto.FullName,
                NationalId = dto.NationalId,
                Phone = dto.Phone,
                Email = dto.Email,
                City = dto.City,
                District = dto.District,
                Age = dto.Age,
                AvailabilityDays = string.Join(",", dto.AvailabilityDays), // تحويل المصفوفة إلى سلسلة
                Skills = dto.Skills,
                MotivationText = dto.MotivationText,
                AgreementAccepted = dto.AgreementAccepted,
                ApplicationDate = DateTime.Now,
                Status = "Pending"
            };

            _context.VolunteerApplications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "تم تقديم الطلب بنجاح" });
        }
    }
}