using System;

namespace GymTurf.Application.DTOs;

public class CreateBookingDto
{
    public Guid FacilityId { get; set; }
    public DateTime BookingDate { get; set; }
    public string StartTime { get; set; } = string.Empty; // "HH:mm"
    public string EndTime { get; set; } = string.Empty;   // "HH:mm"
    public string Notes { get; set; } = string.Empty;
}

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid FacilityId { get; set; }
    public string FacilityName { get; set; } = string.Empty;
    public string FacilityType { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
