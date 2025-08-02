namespace LoveForTennis.Application.DTOs;

public class BookingPlayerDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string PlayerUserId { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? LastUpdated { get; set; }

    // Additional properties for display
    public string PlayerUserName { get; set; } = string.Empty;
}