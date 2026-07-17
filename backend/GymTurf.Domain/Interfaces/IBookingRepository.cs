using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Domain.Entities;

namespace GymTurf.Domain.Interfaces;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id);
    Task<IEnumerable<Booking>> GetByFacilityAndDateAsync(Guid facilityId, DateTime date);
    Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task CreateAsync(Booking booking);
    Task UpdateAsync(Booking booking);
}
