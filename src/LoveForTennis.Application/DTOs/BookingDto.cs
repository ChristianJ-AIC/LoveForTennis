using LoveForTennis.Core.Enums;

namespace LoveForTennis.Application.DTOs;

public class BookingDto
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

    // Additional properties for display
    public string BookedByUserName { get; set; } = string.Empty;
    public string CourtName { get; set; } = string.Empty;
    public ICollection<BookingPlayerDto> Players { get; set; } = new List<BookingPlayerDto>();
}