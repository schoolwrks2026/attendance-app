using GymTurf.Domain.Entities;

namespace GymTurf.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
