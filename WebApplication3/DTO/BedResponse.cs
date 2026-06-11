namespace WebApplication3.DTO;

public class BedResponse
{
    public int Id { get; set; }
    public BedTypeResponse BedType { get; set; } = null!;
    public RoomResponse Room { get; set; } = null!;
}