using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger _logger;

    public AuthService(UserManager<AppUser> userManager,
                       SignInManager<AppUser> signInManager,
                       IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = Log.ForContext<AuthService>();
    }

    public async Task<string> RegisterAsync(RegisterUserDto dto)
    {
        _logger.Information("Attempting to register user with username: {Username} and email: {Email}", dto.Username, dto.Email);

        var user = new AppUser
        {
            UserName = dto.Username,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            _logger.Warning("Registration failed for user {Username}: {Errors}", dto.Username, string.Join("; ", result.Errors.Select(e => e.Description)));
            throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        _logger.Information("User {Username} registered successfully", dto.Username);
        return _jwtTokenService.GenerateToken(user);
    }

    public async Task<string> LoginAsync(LoginUserDto dto)
    {
        _logger.Information("Attempting login for {Login}", dto.Login);

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
            _logger.Warning("Login failed: No user found for login: {Login}", dto.Login);
            throw new UnauthorizedAccessException("Invalid login attempt.");
        }

        // Check the password using CheckPasswordSignInAsync
        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

        if (!result.Succeeded)
        {
            _logger.Warning("Login failed for user {Login}: Invalid password", dto.Login);
            throw new UnauthorizedAccessException("Invalid login attempt.");
        }

        _logger.Information("User {Login} logged in successfully", dto.Login);
        return _jwtTokenService.GenerateToken(user);
    }
}
