using System;
using GymTurf.Domain.Common;

namespace GymTurf.Domain.Entities;

public class MemberProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public string MembershipType { get; set; } = string.Empty; // None, Monthly, Annual, Premium
    public DateTime MembershipStartDate { get; set; }
    public DateTime MembershipEndDate { get; set; }
    public bool IsMembershipActive { get; set; } = false;
    public string MedicalNotes { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
}
