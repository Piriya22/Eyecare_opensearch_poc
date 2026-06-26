namespace EyeCareSearch.API.Models
{
    public class Patient
    {
        public string PatientId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public string Doctor { get; set; } = string.Empty;

        public string Diagnosis { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public DateTime AppointmentDate { get; set; }

        public string? HighlightName { get; set; }

        public string? HighlightDoctor { get; set; }

        public string? HighlightDiagnosis { get; set; }
    }
}
