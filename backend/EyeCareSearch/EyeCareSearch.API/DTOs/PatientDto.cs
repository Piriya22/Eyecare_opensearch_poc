namespace EyeCareSearch.API.DTOs;

public class PatientDto
{
    public string PatientId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public string Doctor { get; set; } = string.Empty;

    public string Diagnosis { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public DateTime AppointmentDate { get; set; }
}