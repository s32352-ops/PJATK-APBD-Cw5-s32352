namespace WebApplication3.DTO;

public class AdmissionResponse
{
    public int Id { get; set; }
    public DateTime AdmissionDate { get; set; }
    public DateTime? DischargeDate { get; set; }
    public WardResponse Ward { get; set; } = null!;
}