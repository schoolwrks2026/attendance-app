using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(
        IBookingRepository bookingRepository,
        IFacilityRepository facilityRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _facilityRepository = facilityRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingDto> CreateBookingAsync(Guid userId, CreateBookingDto dto)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var facility = await _facilityRepository.GetByIdAsync(dto.FacilityId);
            if (facility == null) throw new Exception("Target Facility does not exist.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User account not found.");

            var start = TimeSpan.Parse(dto.StartTime);
            var end = TimeSpan.Parse(dto.EndTime);

            if (start >= end) throw new Exception("Start time must be before end time.");

            // Check for booking overlap to prevent double bookings
            var existingBookings = await _bookingRepository.GetByFacilityAndDateAsync(dto.FacilityId, dto.BookingDate);
            foreach (var b in existingBookings)
            {
                if (b.StartTime < end && b.EndTime > start)
                {
                    throw new Exception("Conflict detected: This time slot is already booked for this facility.");
                }
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                FacilityId = dto.FacilityId,
                UserId = userId,
                MemberName = user.FullName,
                BookingDate = dto.BookingDate.Date,
                StartTime = start,
                EndTime = end,
                Status = "Confirmed",
                Notes = dto.Notes
            };

            await _bookingRepository.CreateAsync(booking);
            _unitOfWork.Commit();

            return MapToDto(booking, facility);
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetMyBookingsAsync(Guid userId)
    {
        var bookings = await _bookingRepository.GetByUserIdAsync(userId);
        var list = new List<BookingDto>();
        foreach (var b in bookings)
        {
            var fac = await _facilityRepository.GetByIdAsync(b.FacilityId);
            if (fac != null)
            {
                list.Add(MapToDto(b, fac));
            }
        }
        return list;
    }

    public async Task<IEnumerable<BookingDto>> GetFacilityBookingsAsync(Guid facilityId, DateTime date)
    {
        var fac = await _facilityRepository.GetByIdAsync(facilityId);
        if (fac == null) return Array.Empty<BookingDto>();

        var bookings = await _bookingRepository.GetByFacilityAndDateAsync(facilityId, date);
        return bookings.Select(b => MapToDto(b, fac));
    }

    public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        var list = new List<BookingDto>();
        foreach (var b in bookings)
        {
            var fac = await _facilityRepository.GetByIdAsync(b.FacilityId);
            if (fac != null)
            {
                list.Add(MapToDto(b, fac));
            }
        }
        return list;
    }

    public async Task CancelBookingAsync(Guid id, Guid userId, string role)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) throw new Exception("Booking record not found.");

            // Allow canceling if user owns the booking, or holds Admin/Manager role
            if (booking.UserId != userId && role != "Super Admin" && role != "System Owner" && role != "Branch Manager")
            {
                throw new Exception("Unauthorized: You do not have permission to cancel this booking.");
            }

            booking.Status = "Cancelled";
            booking.UpdatedAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    private static BookingDto MapToDto(Booking b, Facility fac)
    {
        return new BookingDto
        {
            Id = b.Id,
            FacilityId = b.FacilityId,
            FacilityName = fac.Name,
            FacilityType = fac.Type,
            UserId = b.UserId,
            MemberName = b.MemberName,
            BookingDate = b.BookingDate,
            StartTime = b.StartTime.ToString(@"hh\:mm"),
            EndTime = b.EndTime.ToString(@"hh\:mm"),
            Status = b.Status,
            Notes = b.Notes
        };
    }
}
