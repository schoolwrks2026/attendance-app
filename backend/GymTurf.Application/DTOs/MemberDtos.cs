using System;

namespace GymTurf.Application.DTOs;

public class UpdateMemberProfileDto
{
    public string MedicalNotes { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
}

public class PurchaseMembershipDto
{
    public string MembershipType { get; set; } = string.Empty; // Monthly, Annual, Premium
    public int DurationInMonths { get; set; }
}

public class MemberProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MembershipType { get; set; } = string.Empty;
    public DateTime MembershipStartDate { get; set; }
    public DateTime MembershipEndDate { get; set; }
    public bool IsMembershipActive { get; set; }
    public string MedicalNotes { get; set; } = string.Empty;
    public string EmergencyContactName { get; set; } = string.Empty;
    public string EmergencyContactPhone { get; set; } = string.Empty;
}
