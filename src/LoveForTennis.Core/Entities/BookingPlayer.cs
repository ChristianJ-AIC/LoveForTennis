namespace LoveForTennis.Core.Entities;

public class BookingPlayer
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string PlayerUserId { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime? LastUpdated { get; set; }

    // Navigation properties
    public Booking Booking { get; set; } = null!;
    public ApplicationUser PlayerUser { get; set; } = null!;
}