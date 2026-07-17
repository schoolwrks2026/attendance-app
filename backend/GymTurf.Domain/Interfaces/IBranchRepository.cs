using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Domain.Entities;

namespace GymTurf.Domain.Interfaces;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(Guid id);
    Task<IEnumerable<Branch>> GetAllActiveAsync();
    Task CreateAsync(Branch branch);
    Task UpdateAsync(Branch branch);
    Task DeleteAsync(Guid id);
}
