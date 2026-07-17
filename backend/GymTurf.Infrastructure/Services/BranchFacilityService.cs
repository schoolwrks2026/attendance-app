using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Services;

public class BranchFacilityService : IBranchFacilityService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IFacilityRepository _facilityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BranchFacilityService(
        IBranchRepository branchRepository,
        IFacilityRepository facilityRepository,
        IUnitOfWork unitOfWork)
    {
        _branchRepository = branchRepository;
        _facilityRepository = facilityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
    {
        return await _branchRepository.GetAllActiveAsync();
    }

    public async Task<Branch> CreateBranchAsync(Branch branch)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _branchRepository.CreateAsync(branch);
            _unitOfWork.Commit();
            return branch;
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task UpdateBranchAsync(Branch branch)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _branchRepository.UpdateAsync(branch);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task DeleteBranchAsync(Guid id)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _branchRepository.DeleteAsync(id);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<IEnumerable<Facility>> GetFacilitiesByBranchAsync(Guid branchId)
    {
        return await _facilityRepository.GetByBranchIdAsync(branchId);
    }

    public async Task<Facility> CreateFacilityAsync(Facility facility)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var branch = await _branchRepository.GetByIdAsync(facility.BranchId);
            if (branch == null)
            {
                throw new Exception("Target Branch does not exist.");
            }

            await _facilityRepository.CreateAsync(facility);
            _unitOfWork.Commit();
            return facility;
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task UpdateFacilityAsync(Facility facility)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _facilityRepository.UpdateAsync(facility);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task DeleteFacilityAsync(Guid id)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            await _facilityRepository.DeleteAsync(id);
            _unitOfWork.Commit();
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }
}
