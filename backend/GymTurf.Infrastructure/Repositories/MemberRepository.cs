using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public MemberRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MemberProfile?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM ""MemberProfiles""
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<MemberProfile>(
            query, new { Id = id }, _unitOfWork.Transaction);
    }

    public async Task<MemberProfile?> GetByUserIdAsync(Guid userId)
    {
        const string query = @"
            SELECT * FROM ""MemberProfiles""
            WHERE ""UserId"" = @UserId AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<MemberProfile>(
            query, new { UserId = userId }, _unitOfWork.Transaction);
    }

    public async Task<IEnumerable<MemberProfile>> GetAllAsync()
    {
        const string query = @"
            SELECT * FROM ""MemberProfiles""
            WHERE ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryAsync<MemberProfile>(
            query, null, _unitOfWork.Transaction);
    }

    public async Task CreateAsync(MemberProfile profile)
    {
        const string sql = @"
            INSERT INTO ""MemberProfiles"" (
                ""Id"", ""TenantId"", ""UserId"", ""MembershipType"", ""MembershipStartDate"",
                ""MembershipEndDate"", ""IsMembershipActive"", ""MedicalNotes"",
                ""EmergencyContactName"", ""EmergencyContactPhone"", ""CreatedAt"",
                ""UpdatedAt"", ""CreatedBy"", ""UpdatedBy"", ""Version""
            ) VALUES (
                @Id, @TenantId, @UserId, @MembershipType, @MembershipStartDate,
                @MembershipEndDate, @IsMembershipActive, @MedicalNotes,
                @EmergencyContactName, @EmergencyContactPhone, @CreatedAt,
                @UpdatedAt, @CreatedBy, @UpdatedBy, @Version
            );";
        await _unitOfWork.Connection.ExecuteAsync(sql, profile, _unitOfWork.Transaction);
    }

    public async Task UpdateAsync(MemberProfile profile)
    {
        const string sql = @"
            UPDATE ""MemberProfiles"" SET
                ""MembershipType"" = @MembershipType,
                ""MembershipStartDate"" = @MembershipStartDate,
                ""MembershipEndDate"" = @MembershipEndDate,
                ""IsMembershipActive"" = @IsMembershipActive,
                ""MedicalNotes"" = @MedicalNotes,
                ""EmergencyContactName"" = @EmergencyContactName,
                ""EmergencyContactPhone"" = @EmergencyContactPhone,
                ""UpdatedAt"" = @UpdatedAt,
                ""UpdatedBy"" = @UpdatedBy,
                ""Version"" = ""Version"" + 1
            WHERE ""Id"" = @Id AND ""Version"" = @Version AND ""DeletedAt"" IS NULL;";

        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, profile, _unitOfWork.Transaction);
        if (rowsAffected == 0)
        {
            throw new Exception("Concurrency conflict detected during Member Profile update.");
        }
        profile.Version++;
    }
}
