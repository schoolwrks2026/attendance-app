using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Domain.Entities;

namespace GymTurf.Domain.Interfaces;

public interface IMemberRepository
{
    Task<MemberProfile?> GetByIdAsync(Guid id);
    Task<MemberProfile?> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<MemberProfile>> GetAllAsync();
    Task CreateAsync(MemberProfile profile);
    Task UpdateAsync(MemberProfile profile);
}
