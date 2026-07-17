using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Common;
using FluentValidation;
using System;
using System.Threading.Tasks;

namespace GymTurf.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BranchController : ControllerBase
{
    private readonly IBranchFacilityService _branchFacilityService;
    private readonly IValidator<CreateBranchDto> _branchValidator;
    private readonly IValidator<CreateFacilityDto> _facilityValidator;

    public BranchController(
        IBranchFacilityService branchFacilityService,
        IValidator<CreateBranchDto> branchValidator,
        IValidator<CreateFacilityDto> facilityValidator)
    {
        _branchFacilityService = branchFacilityService;
        _branchValidator = branchValidator;
        _facilityValidator = facilityValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetBranches()
    {
        var branches = await _branchFacilityService.GetActiveBranchesAsync();
        return Ok(branches);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.SystemOwner}")]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchDto dto)
    {
        var validationResult = await _branchValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true
        };

        var created = await _branchFacilityService.CreateBranchAsync(branch);
        return Ok(created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.SystemOwner}")]
    public async Task<IActionResult> DeleteBranch(Guid id)
    {
        await _branchFacilityService.DeleteBranchAsync(id);
        return NoContent();
    }

    [HttpGet("{branchId}/facilities")]
    public async Task<IActionResult> GetFacilities(Guid branchId)
    {
        var facilities = await _branchFacilityService.GetFacilitiesByBranchAsync(branchId);
        return Ok(facilities);
    }

    [HttpPost("facilities")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.SystemOwner},{UserRoles.BranchManager}")]
    public async Task<IActionResult> CreateFacility([FromBody] CreateFacilityDto dto)
    {
        var validationResult = await _facilityValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var facility = new Facility
        {
            Id = Guid.NewGuid(),
            BranchId = dto.BranchId,
            Name = dto.Name,
            Type = dto.Type,
            Capacity = dto.Capacity,
            PricePerHour = dto.PricePerHour,
            IsActive = true
        };

        var created = await _branchFacilityService.CreateFacilityAsync(facility);
        return Ok(created);
    }

    [HttpDelete("facilities/{id}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.SystemOwner},{UserRoles.BranchManager}")]
    public async Task<IActionResult> DeleteFacility(Guid id)
    {
        await _branchFacilityService.DeleteFacilityAsync(id);
        return NoContent();
    }
}
