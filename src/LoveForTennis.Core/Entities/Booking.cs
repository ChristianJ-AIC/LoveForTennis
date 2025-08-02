namespace LoveForTennis.Core.Entities;

public class Booking
{
    public int Id { get; set; }
    public string BookedByUserId { get; set; } = string.Empty;
    public DateTime BookingFrom { get; set; }
    public DateTime BookingTo { get; set; }
    public bool Cancelled { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastUpdated { get; set; }

    // Navigation properties
    public ApplicationUser BookedByUser { get; set; } = null!;
    public ICollection<BookingPlayer> Players { get; set; } = new List<BookingPlayer>();
}