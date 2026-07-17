using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Domain.Entities;

namespace GymTurf.Domain.Interfaces;

public interface IFacilityRepository
{
    Task<Facility?> GetByIdAsync(Guid id);
    Task<IEnumerable<Facility>> GetByBranchIdAsync(Guid branchId);
    Task CreateAsync(Facility facility);
    Task UpdateAsync(Facility facility);
    Task DeleteAsync(Guid id);
}
