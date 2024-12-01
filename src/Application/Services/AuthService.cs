using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<AppUser> userManager,
                       SignInManager<AppUser> signInManager,
                       IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<string> RegisterAsync(RegisterUserDto dto)
    {
        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        return _jwtTokenService.GenerateToken(user);
    }

    public async Task<string> LoginAsync(LoginUserDto dto)
    {
        // Check if the input is an email or a username
        var user = await _userManager.FindByEmailAsync(dto.Login);
        if (user == null)
        {
            // If no user found with email, try finding by username
            user = await _userManager.FindByNameAsync(dto.Login);
        }

        // If no user found by either email or username
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid login attempt.");
        }

        // Check the password using CheckPasswordSignInAsync
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid login attempt.");
        }

        return _jwtTokenService.GenerateToken(user);
    }
}