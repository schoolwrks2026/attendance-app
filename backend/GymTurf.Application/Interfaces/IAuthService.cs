using System.Threading.Tasks;
using GymTurf.Application.DTOs;

namespace GymTurf.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
