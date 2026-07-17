using System;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;
using GymTurf.Infrastructure.Services;
using Moq;
using Xunit;

namespace GymTurf.Tests;

public class MemberServiceTests
{
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly MemberService _service;

    public MemberServiceTests()
    {
        _memberRepoMock = new Mock<IMemberRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _service = new MemberService(_memberRepoMock.Object, _userRepoMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task PurchaseMembership_ShouldActivateSubscription_WithCorrectDates()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FullName = "Bob Smith", Email = "bob@example.com" };
        var profile = new MemberProfile { UserId = userId, MembershipType = "None" };

        _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
        _memberRepoMock.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(profile);

        var dto = new PurchaseMembershipDto { MembershipType = "Premium", DurationInMonths = 3 };

        // Act
        var result = await _service.PurchaseMembershipAsync(userId, dto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsMembershipActive);
        Assert.Equal("Premium", result.MembershipType);
        Assert.True(result.MembershipEndDate > DateTime.UtcNow);
        _memberRepoMock.Verify(x => x.UpdateAsync(profile), Times.Once);
        _uowMock.Verify(x => x.Commit(), Times.Once);
    }
}
