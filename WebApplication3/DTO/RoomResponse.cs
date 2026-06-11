namespace WebApplication3.DTO;

public class RoomResponse
{
    public string Id { get; set; } = null!;
    public bool HasTv { get; set; }
    public WardResponse Ward { get; set; } = null!;
}