using LoveForTennis.Core.Enums;

namespace LoveForTennis.Core.Entities;

public class Court
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CourtSurfaceType SurfaceType { get; set; }
    public BookingTimeType AllowedBookingTimeType { get; set; }
    public InOrOutdoorType InOrOutdoorType { get; set; }
    public TimeOnly BookingAllowedFrom { get; set; }
    public TimeOnly BookingAllowedTill { get; set; }
    public int? BookingsOpenForNumberOfDaysIntoTheFuture { get; set; }
}