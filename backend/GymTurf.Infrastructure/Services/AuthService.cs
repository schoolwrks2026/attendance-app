using System;
using System.Threading.Tasks;
using GymTurf.Application.DTOs;
using GymTurf.Application.Interfaces;
using GymTurf.Domain.Entities;
using GymTurf.Domain.Interfaces;

namespace GymTurf.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUserRepository userRepository, IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        _unitOfWork.BeginTransaction();
        try
        {
            var existingUserByEmail = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
            {
                throw new Exception("Email is already registered.");
            }

            var existingUserByUsername = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUserByUsername != null)
            {
                throw new Exception("Username is already taken.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                IsActive = true
            };

            await _userRepository.CreateAsync(user);
            _unitOfWork.Commit();

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber
                },
                Token = token
            };
        }
        catch
        {
            _unitOfWork.Rollback();
            throw;
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        User? user = null;
        if (dto.UsernameOrEmail.Contains("@"))
        {
            user = await _userRepository.GetByEmailAsync(dto.UsernameOrEmail);
        }
        else
        {
            user = await _userRepository.GetByUsernameAsync(dto.UsernameOrEmail);
        }

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            throw new Exception("Invalid username/email or password.");
        }

        if (!user.IsActive)
        {
            throw new Exception("User account is inactive.");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthResponseDto
        {
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber
            },
            Token = token
        };
    }
}
