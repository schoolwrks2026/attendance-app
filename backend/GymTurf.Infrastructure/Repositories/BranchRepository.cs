using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public BranchRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Branch?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM ""Branches""
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Branch>(
            query, new { Id = id }, _unitOfWork.Transaction);
    }

    public async Task<IEnumerable<Branch>> GetAllActiveAsync()
    {
        const string query = @"
            SELECT * FROM ""Branches""
            WHERE ""IsActive"" = true AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryAsync<Branch>(
            query, null, _unitOfWork.Transaction);
    }

    public async Task CreateAsync(Branch branch)
    {
        const string sql = @"
            INSERT INTO ""Branches"" (
                ""Id"", ""TenantId"", ""Name"", ""Address"", ""PhoneNumber"",
                ""IsActive"", ""CreatedAt"", ""UpdatedAt"", ""CreatedBy"", ""UpdatedBy"", ""Version""
            ) VALUES (
                @Id, @TenantId, @Name, @Address, @PhoneNumber,
                @IsActive, @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @Version
            );";
        await _unitOfWork.Connection.ExecuteAsync(sql, branch, _unitOfWork.Transaction);
    }

    public async Task UpdateAsync(Branch branch)
    {
        const string sql = @"
            UPDATE ""Branches"" SET
                ""Name"" = @Name,
                ""Address"" = @Address,
                ""PhoneNumber"" = @PhoneNumber,
                ""IsActive"" = @IsActive,
                ""UpdatedAt"" = @UpdatedAt,
                ""UpdatedBy"" = @UpdatedBy,
                ""Version"" = ""Version"" + 1
            WHERE ""Id"" = @Id AND ""Version"" = @Version AND ""DeletedAt"" IS NULL;";

        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, branch, _unitOfWork.Transaction);
        if (rowsAffected == 0)
        {
            throw new Exception("Concurrency conflict detected during Branch update.");
        }
        branch.Version++;
    }

    public async Task DeleteAsync(Guid id)
    {
        const string sql = @"
            UPDATE ""Branches"" SET
                ""DeletedAt"" = @DeletedAt,
                ""UpdatedAt"" = @DeletedAt
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        await _unitOfWork.Connection.ExecuteAsync(sql, new { Id = id, DeletedAt = DateTime.UtcNow }, _unitOfWork.Transaction);
    }
}
