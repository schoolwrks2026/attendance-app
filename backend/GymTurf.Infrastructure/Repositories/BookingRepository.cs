using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingRepository(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Booking?> GetByIdAsync(Guid id)
    {
        const string query = @"
            SELECT * FROM ""Bookings""
            WHERE ""Id"" = @Id AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<Booking>(
            query, new { Id = id }, _unitOfWork.Transaction);
    }

    public async Task<IEnumerable<Booking>> GetByFacilityAndDateAsync(Guid facilityId, DateTime date)
    {
        const string query = @"
            SELECT * FROM ""Bookings""
            WHERE ""FacilityId"" = @FacilityId
              AND ""BookingDate"" = @BookingDate
              AND ""Status"" = 'Confirmed'
              AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryAsync<Booking>(
            query, new { FacilityId = facilityId, BookingDate = date.Date }, _unitOfWork.Transaction);
    }

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(Guid userId)
    {
        const string query = @"
            SELECT * FROM ""Bookings""
            WHERE ""UserId"" = @UserId AND ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryAsync<Booking>(
            query, new { UserId = userId }, _unitOfWork.Transaction);
    }

    public async Task<IEnumerable<Booking>> GetAllAsync()
    {
        const string query = @"
            SELECT * FROM ""Bookings"" WHERE ""DeletedAt"" IS NULL;";
        return await _unitOfWork.Connection.QueryAsync<Booking>(
            query, null, _unitOfWork.Transaction);
    }

    public async Task CreateAsync(Booking booking)
    {
        const string sql = @"
            INSERT INTO ""Bookings"" (
                ""Id"", ""TenantId"", ""FacilityId"", ""UserId"", ""MemberName"",
                ""BookingDate"", ""StartTime"", ""EndTime"", ""Status"", ""Notes"",
                ""CreatedAt"", ""UpdatedAt"", ""CreatedBy"", ""UpdatedBy"", ""Version""
            ) VALUES (
                @Id, @TenantId, @FacilityId, @UserId, @MemberName,
                @BookingDate, @StartTime, @EndTime, @Status, @Notes,
                @CreatedAt, @UpdatedAt, @CreatedBy, @UpdatedBy, @Version
            );";
        await _unitOfWork.Connection.ExecuteAsync(sql, booking, _unitOfWork.Transaction);
    }

    public async Task UpdateAsync(Booking booking)
    {
        const string sql = @"
            UPDATE ""Bookings"" SET
                ""Status"" = @Status,
                ""Notes"" = @Notes,
                ""UpdatedAt"" = @UpdatedAt,
                ""UpdatedBy"" = @UpdatedBy,
                ""Version"" = ""Version"" + 1
            WHERE ""Id"" = @Id AND ""Version"" = @Version AND ""DeletedAt"" IS NULL;";

        int rowsAffected = await _unitOfWork.Connection.ExecuteAsync(sql, booking, _unitOfWork.Transaction);
        if (rowsAffected == 0)
        {
            throw new Exception("Concurrency conflict detected during Booking update.");
        }
        booking.Version++;
    }
}
