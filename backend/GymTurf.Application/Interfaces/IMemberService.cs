using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;

namespace GymTurf.Application.Interfaces;

public interface IMemberService
{
    Task<MemberProfileDto?> GetProfileByUserIdAsync(Guid userId);
    Task<IEnumerable<MemberProfileDto>> GetAllProfilesAsync();
    Task<MemberProfileDto> UpdateProfileAsync(Guid userId, UpdateMemberProfileDto dto);
    Task<MemberProfileDto> PurchaseMembershipAsync(Guid userId, PurchaseMembershipDto dto);
}
