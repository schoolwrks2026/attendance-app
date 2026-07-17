using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Application.Interfaces;

public interface IBranchFacilityService
{
    Task<IEnumerable<Branch>> GetActiveBranchesAsync();
    Task<Branch> CreateBranchAsync(Branch branch);
    Task UpdateBranchAsync(Branch branch);
    Task DeleteBranchAsync(Guid id);

    Task<IEnumerable<Facility>> GetFacilitiesByBranchAsync(Guid branchId);
    Task<Facility> CreateFacilityAsync(Facility facility);
    Task UpdateFacilityAsync(Facility facility);
    Task DeleteFacilityAsync(Guid id);
}
