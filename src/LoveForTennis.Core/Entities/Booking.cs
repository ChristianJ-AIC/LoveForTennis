using LoveForTennis.Core.Enums;

namespace LoveForTennis.Core.Entities;

public class Booking
{
    public int Id { get; set; }
    public string BookedByUserId { get; set; } = string.Empty;
    public int CourtId { get; set; }
    public DateTime BookingFrom { get; set; }
    public DateTime BookingTo { get; set; }
    public bool Cancelled { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastUpdated { get; set; }
    public BookingType BookingType { get; set; } = BookingType.NotSet;

    // Navigation properties
    public ApplicationUser BookedByUser { get; set; } = null!;
    public Court Court { get; set; } = null!;
    public ICollection<BookingPlayer> Players { get; set; } = new List<BookingPlayer>();
}