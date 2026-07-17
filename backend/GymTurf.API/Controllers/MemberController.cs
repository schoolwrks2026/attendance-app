using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Common;
using FluentValidation;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymTurf.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MemberController : ControllerBase
{
    private readonly IMemberService _memberService;
    private readonly IValidator<UpdateMemberProfileDto> _profileValidator;
    private readonly IValidator<PurchaseMembershipDto> _membershipValidator;

    public MemberController(
        IMemberService memberService,
        IValidator<UpdateMemberProfileDto> profileValidator,
        IValidator<PurchaseMembershipDto> membershipValidator)
    {
        _memberService = memberService;
        _profileValidator = profileValidator;
        _membershipValidator = membershipValidator;
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !Guid.TryParse(claim.Value, out var userId))
        {
            throw new Exception("Unauthorized: Invalid user principal.");
        }
        return userId;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetUserId();
        var profile = await _memberService.GetProfileByUserIdAsync(userId);
        return Ok(profile);
    }

    [HttpPost("me/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateMemberProfileDto dto)
    {
        var validationResult = await _profileValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = GetUserId();
        var updated = await _memberService.UpdateProfileAsync(userId, dto);
        return Ok(updated);
    }

    [HttpPost("me/membership")]
    public async Task<IActionResult> PurchaseMembership([FromBody] PurchaseMembershipDto dto)
    {
        var validationResult = await _membershipValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = GetUserId();
        var updated = await _memberService.PurchaseMembershipAsync(userId, dto);
        return Ok(updated);
    }

    [HttpGet]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.SystemOwner},{UserRoles.BranchManager},{UserRoles.Receptionist}")]
    public async Task<IActionResult> GetAllMembers()
    {
        var list = await _memberService.GetAllProfilesAsync();
        return Ok(list);
    }
}
