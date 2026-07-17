using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Repositories;

public class FacilityRepository : IFacilityRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public FacilityRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Facility?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM ""Facilities""
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Facility>(
            query, new { Id = id }, _unitOfWork.Transaction);
    }

    public async Task<IEnumerable<Facility>> GetByBranchIdAsync(Guid branchId)
    {
        const string query = @"
            SELECT * FROM ""Facilities""
            WHERE ""BranchId"" = @BranchId AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryAsync<Facility>(
            query, new { BranchId = branchId }, _unitOfWork.Transaction);
    }

    public async Task CreateAsync(Facility facility)
    {
        const string sql = @"
            INSERT INTO ""Facilities"" (
                ""Id"", ""TenantId"", ""BranchId"", ""Name"", ""Type"", ""Capacity"",
                ""PricePerHour"", ""IsActive"", ""CreatedAt"", ""UpdatedAt"",
                ""CreatedBy"", ""UpdatedBy"", ""Version""
            ) VALUES (
                @Id, @TenantId, @BranchId, @Name, @Type, @Capacity,
                @PricePerHour, @IsActive, @CreatedAt, @UpdatedAt,
                @CreatedBy, @UpdatedBy, @Version
            );";
        await _unitOfWork.Connection.ExecuteAsync(sql, facility, _unitOfWork.Transaction);
    }

    public async Task UpdateAsync(Facility facility)
    {
        const string sql = @"
            UPDATE ""Facilities"" SET
                ""Name"" = @Name,
                ""Type"" = @Type,
                ""Capacity"" = @Capacity,
                ""PricePerHour"" = @PricePerHour,
                ""IsActive"" = @IsActive,
                ""UpdatedAt"" = @UpdatedAt,
                ""UpdatedBy"" = @UpdatedBy,
                ""Version"" = ""Version"" + 1
            WHERE ""Id"" = @Id AND ""Version"" = @Version AND ""DeletedAt"" IS NULL;";

        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, facility, _unitOfWork.Transaction);
        if (rowsAffected == 0)
        {
            throw new Exception("Concurrency conflict detected during Facility update.");
        }
        facility.Version++;
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"
            UPDATE ""Facilities"" SET
                ""DeletedAt"" = @DeletedAt,
                ""UpdatedAt"" = @DeletedAt
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        await _unitOfWork.Connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow }, _unitOfWork.Transaction);
    }
}
