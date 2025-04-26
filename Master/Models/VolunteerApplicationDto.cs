namespace Master.Models
{
    public class VolunteerApplicationDto
    {
        public string FullName { get; set; }
        public string? NationalId { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public int Age { get; set; }
        public List<string> AvailabilityDays { get; set; } // تبقى كمصفوفة في DTO
        public string? Skills { get; set; }
        public string MotivationText { get; set; }
        public bool AgreementAccepted { get; set; }
    }
}
