using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;

namespace GymTurf.Application.Interfaces;

public interface IBookingService
{
    Task<BookingDto> CreateBookingAsync(Guid userId, CreateBookingDto dto);
    Task<IEnumerable<BookingDto>> GetMyBookingsAsync(Guid userId);
    Task<IEnumerable<BookingDto>> GetFacilityBookingsAsync(Guid facilityId, DateTime date);
    Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
    Task CancelBookingAsync(Guid id, Guid userId, string role);
}
