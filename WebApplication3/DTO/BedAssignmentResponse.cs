namespace WebApplication3.DTO;

public class BedAssignmentResponse
{
    public int Id { get; set; }
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
    public BedResponse Bed { get; set; } = null!;
}