using Application.DTOs;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterUserDto dto);
    Task<string> LoginAsync(LoginUserDto dto);
}