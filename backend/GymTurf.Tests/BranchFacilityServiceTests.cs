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

public class BranchFacilityServiceTests
{
    private readonly Mock<IBranchRepository> _branchRepositoryMock;
    private readonly Mock<IFacilityRepository> _facilityRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BranchFacilityService _service;

    public BranchFacilityServiceTests()
    {
        _branchRepositoryMock = new Mock<IBranchRepository>();
        _facilityRepositoryMock = new Mock<IFacilityRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new BranchFacilityService(
            _branchRepositoryMock.Object,
            _facilityRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task CreateBranch_ShouldSaveToRepository_WhenDetailsAreValid()
    {
        // Arrange
        var branch = new Branch { Name = "Main Branch", Address = "123 Main St" };

        // Act
        var result = await _service.CreateBranchAsync(branch);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Main Branch", result.Name);
        _branchRepositoryMock.Verify(x => x.CreateAsync(branch), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateFacility_ShouldThrowException_WhenBranchDoesNotExist()
    {
        // Arrange
        var facility = new Facility { BranchId = Guid.NewGuid(), Name = "Gym Room" };
        _branchRepositoryMock.Setup(x => x.GetByIdAsync(facility.BranchId))
            .ReturnsAsync((Branch?)null);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.CreateFacilityAsync(facility));
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
    }
}
