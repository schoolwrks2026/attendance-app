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

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _jwtTokenGeneratorMock.Object
        );
    }

    [Fact]
    public async Task Register_ShouldCreateUser_WhenDetailsAreValid()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "SecurePassword123!",
            FullName = "Test User",
            Role = "Member"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(registerDto.Username))
            .ReturnsAsync((User?)null);
        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");

        // Act
        var result = await _authService.RegisterAsync(registerDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.User.Username);
        Assert.Equal("fake-jwt-token", result.Token);
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDto { Email = "exists@example.com" };
        _userRepositoryMock.Setup(x => x.GetByEmailAsync(registerDto.Email))
            .ReturnsAsync(new User());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
    }
}
