using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;
using GymTurf.Infrastructure.Services;
using Moq;
using Xunit;

namespace GymTurf.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IFacilityRepository> _facilityRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _bookingRepoMock = new Mock<IBookingRepository>();
        _facilityRepoMock = new Mock<IFacilityRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _service = new BookingService(
            _bookingRepoMock.Object,
            _facilityRepoMock.Object,
            _userRepoMock.Object,
            _uowMock.Object
        );
    }

    [Fact]
    public async Task CreateBooking_ShouldThrowException_WhenOverlapDetected()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var facId = Guid.NewGuid();
        var bookingDate = DateTime.UtcNow.Date.AddDays(1);

        var existingBooking = new Booking
        {
            FacilityId = facId,
            BookingDate = bookingDate,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(15, 0, 0),
            Status = "Confirmed"
        };

        _facilityRepoMock.Setup(x => x.GetByIdAsync(facId)).ReturnsAsync(new Facility());
        _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(new User { FullName = "Alice" });
        _bookingRepoMock.Setup(x => x.GetByFacilityAndDateAsync(facId, bookingDate))
            .ReturnsAsync(new List<Booking> { existingBooking });

        var newBookingDto = new CreateBookingDto
        {
            FacilityId = facId,
            BookingDate = bookingDate,
            StartTime = "14:30",
            EndTime = "15:30"
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateBookingAsync(userId, newBookingDto));
        Assert.Contains("Conflict detected", ex.Message);
        _uowMock.Verify(x => x.Commit(), Times.Never);
    }
}
