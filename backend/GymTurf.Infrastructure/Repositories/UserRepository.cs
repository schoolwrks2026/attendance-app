using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public UserRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM ""Users""
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
            query, new { Id = id }, _unitOfWork.Transaction);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        const string query = @"
            SELECT * FROM ""Users""
            WHERE ""Username"" = @Username AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
            query, new { Username = username }, _unitOfWork.Transaction);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        const string query = @"
            SELECT * FROM ""Users""
            WHERE ""Email"" = @Email AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<User>(
            query, new { Email = email }, _unitOfWork.Transaction);
    }

    public async Task CreateAsync(User user)
    {
        const string sql = @"
            INSERT INTO ""Users"" (
                ""Id"", ""TenantId"", ""Username"", ""Email"", ""PasswordHash"",
                ""Role"", ""FullName"", ""PhoneNumber"", ""IsActive"",
                ""CreatedAt"", ""UpdatedAt"", ""CreatedBy"", ""UpdatedBy"", ""Version""
            ) VALUES (
                @Id, @TenantId, @Username, @Email, @PasswordHash,
                @Role, @FullName, @PhoneNumber, @IsActive,
                @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @Version
            );";
        await _unitOfWork.Connection.ExecuteAsync(sql, user, _unitOfWork.Transaction);
    }

    public async Task UpdateAsync(User user)
    {
        const string sql = @"
            UPDATE ""Users"" SET
                ""Username"" = @Username,
                ""Email"" = @Email,
                ""PasswordHash"" = @PasswordHash,
                ""Role"" = @Role,
                ""FullName"" = @FullName,
                ""PhoneNumber"" = @PhoneNumber,
                ""IsActive"" = @IsActive,
                ""UpdatedAt"" = @UpdatedAt,
                ""UpdatedBy"" = @UpdatedBy,
                ""Version"" = ""Version"" + 1
            WHERE ""Id"" = @Id AND ""Version"" = @Version AND ""DeletedAt"" IS NULL;";

        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, user, _unitOfWork.Transaction);
        if (rowsAffected == 0)
        {
            throw new Exception("Concurrency conflict detected during User update, or user has been soft-deleted.");
        }
        user.Version++;
    }
}
