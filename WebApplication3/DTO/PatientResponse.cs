namespace WebApplication3.DTO;

public class PatientResponse
{
    public string Pesel { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public int Age { get; set; }
    public string Sex { get; set; } = null!;
    public List<AdmissionResponse> Admissions { get; set; } = new();
    public List<BedAssignmentResponse> BedAssignments { get; set; } = new();
}