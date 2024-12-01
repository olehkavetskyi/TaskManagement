using Infrastructure.Data.Identity;

namespace Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(AppUser user);
}
