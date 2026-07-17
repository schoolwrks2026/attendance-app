using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using FluentValidation;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymTurf.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IValidator<CreateBookingDto> _bookingValidator;

    public BookingController(
        IBookingService bookingService,
        IValidator<CreateBookingDto> bookingValidator)
    {
        _bookingService = bookingService;
        _bookingValidator = bookingValidator;
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

    private string GetUserRole()
    {
        var claim = User.FindFirst(ClaimTypes.Role);
        return claim?.Value ?? string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var validationResult = await _bookingValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = GetUserId();
        var booking = await _bookingService.CreateBookingAsync(userId, dto);
        return Ok(booking);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = GetUserId();
        var bookings = await _bookingService.GetMyBookingsAsync(userId);
        return Ok(bookings);
    }

    [HttpGet("facility/{facilityId}")]
    public async Task<IActionResult> GetFacilityBookings(Guid facilityId, [FromQuery] DateTime date)
    {
        var list = await _bookingService.GetFacilityBookingsAsync(facilityId, date == default ? DateTime.UtcNow.Date : date);
        return Ok(list);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var list = await _bookingService.GetAllBookingsAsync();
        return Ok(list);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var userId = GetUserId();
        var role = GetUserRole();
        await _bookingService.CancelBookingAsync(id, userId, role);
        return NoContent();
    }
}
