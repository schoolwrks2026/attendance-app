using System;
using GymTurf.Domain.Common;

namespace GymTurf.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid FacilityId { get; set; }
    public Guid UserId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = "Confirmed"; // Confirmed, Cancelled
    public string Notes { get; set; } = string.Empty;
}
