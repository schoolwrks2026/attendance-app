using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MemberService(
        IMemberRepository memberRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _memberRepository = memberRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    private async Task<MemberProfile> GetOrCreateProfileAsync(Guid userId)
    {
        var profile = await _memberRepository.GetByUserIdAsync(userId);
        if (profile == null)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User account not found.");
            }

            profile = new MemberProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                MembershipType = "None",
                MembershipStartDate = DateTime.UtcNow,
                MembershipEndDate = DateTime.UtcNow,
                IsMembershipActive = false,
                MedicalNotes = string.Empty,
                EmergencyContactName = string.Empty,
                EmergencyContactPhone = string.Empty
            };

            await _memberRepository.CreateAsync(profile);
        }
        return profile;
    }

    public async Task<MemberProfileDto?> GetProfileByUserIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return null;

        var profile = await GetOrCreateProfileAsync(userId);

        return MapToDto(profile, user);
    }

    public async Task<IEnumerable<MemberProfileDto>> GetAllProfilesAsync()
    {
        var profiles = await _memberRepository.GetAllAsync();
        var dtos = new List<MemberProfileDto>();

        foreach (var profile in profiles)
        {
            var user = await _userRepository.GetByIdAsync(profile.UserId);
            if (user != null)
            {
                dtos.Add(MapToDto(profile, user));
            }
        }

        return dtos;
    }

    public async Task<MemberProfileDto> UpdateProfileAsync(Guid userId, UpdateMemberProfileDto dto)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");

            var profile = await GetOrCreateProfileAsync(userId);
            profile.MedicalNotes = dto.MedicalNotes;
            profile.EmergencyContactName = dto.EmergencyContactName;
            profile.EmergencyContactPhone = dto.EmergencyContactPhone;
            profile.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(profile);
            _unitOfWork.Commit();

            return MapToDto(profile, user);
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<MemberProfileDto> PurchaseMembershipAsync(Guid userId, PurchaseMembershipDto dto)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found.");

            var profile = await GetOrCreateProfileAsync(userId);
            profile.MembershipType = dto.MembershipType;
            profile.MembershipStartDate = DateTime.UtcNow;
            profile.MembershipEndDate = DateTime.UtcNow.AddMonths(dto.DurationInMonths);
            profile.IsMembershipActive = true;
            profile.UpdatedAt = DateTime.UtcNow;

            await _memberRepository.UpdateAsync(profile);
            _unitOfWork.Commit();

            return MapToDto(profile, user);
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    private static MemberProfileDto MapToDto(MemberProfile profile, User user)
    {
        return new MemberProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = user.FullName,
            Email = user.Email,
            MembershipType = profile.MembershipType,
            MembershipStartDate = profile.MembershipStartDate,
            MembershipEndDate = profile.MembershipEndDate,
            IsMembershipActive = profile.IsMembershipActive && profile.MembershipEndDate > DateTime.UtcNow,
            MedicalNotes = profile.MedicalNotes,
            EmergencyContactName = profile.EmergencyContactName,
            EmergencyContactPhone = profile.EmergencyContactPhone
        };
    }
}
