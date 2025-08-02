namespace LoveForTennis.Application.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public string BookedByUserId { get; set; } = string.Empty;
    public DateTime BookingFrom { get; set; }
    public DateTime BookingTo { get; set; }
    public bool Cancelled { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastUpdated { get; set; }

    // Additional properties for display
    public string BookedByUserName { get; set; } = string.Empty;
    public ICollection<BookingPlayerDto> Players { get; set; } = new List<BookingPlayerDto>();
}