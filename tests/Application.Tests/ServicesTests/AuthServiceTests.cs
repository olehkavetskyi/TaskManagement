using Application.DTOs;
using Application.Interfaces;
using Application.Services;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests;

public class AuthServiceTests
{
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserManager = new Mock<UserManager<AppUser>>(
            new Mock<IUserStore<AppUser>>().Object,
            null, null, null, null, null, null, null, null);

        _mockSignInManager = new Mock<SignInManager<AppUser>>(
            _mockUserManager.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
            null, null, null, null);

        _mockJwtTokenService = new Mock<IJwtTokenService>();

        _authService = new AuthService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockJwtTokenService.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnToken_WhenRegistrationIsSuccessful()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "StrongPassword123!"
        };
        var appUser = new AppUser { UserName = dto.Username, Email = dto.Email };
        var expectedToken = "mock-token";

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _mockJwtTokenService.Setup(x => x.GenerateToken(It.IsAny<AppUser>())).Returns(expectedToken);

        // Act
        var result = await _authService.RegisterAsync(dto);

        // Assert
        Assert.Equal(expectedToken, result);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password), Times.Once);
        _mockJwtTokenService.Verify(x => x.GenerateToken(It.IsAny<AppUser>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenRegistrationFails()
    {
        // Arrange
        var dto = new RegisterUserDto
        {
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "WeakPassword"
        };
        var identityErrors = new[] { new IdentityError { Description = "Password is too weak." } };
        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(dto));
        Assert.Equal("Password is too weak.", exception.Message);
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<AppUser>(), dto.Password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenLoginIsSuccessful()
    {
        // Arrange
        var dto = new LoginUserDto { Login = "testuser", Password = "CorrectPassword123!" };
        var appUser = new AppUser { UserName = dto.Login };
        var expectedToken = "mock-token";

        _mockUserManager.Setup(x => x.FindByEmailAsync(dto.Login)).ReturnsAsync((AppUser)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(dto.Login)).ReturnsAsync(appUser);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(appUser, dto.Password, false))
            .ReturnsAsync(SignInResult.Success);
        _mockJwtTokenService.Setup(x => x.GenerateToken(appUser)).Returns(expectedToken);

        // Act
        var result = await _authService.LoginAsync(dto);

        // Assert
        Assert.Equal(expectedToken, result);
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(appUser, dto.Password, false), Times.Once);
        _mockJwtTokenService.Verify(x => x.GenerateToken(appUser), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenUserNotFound()
    {
        // Arrange
        var dto = new LoginUserDto { Login = "nonexistentuser", Password = "Password123!" };

        _mockUserManager.Setup(x => x.FindByEmailAsync(dto.Login)).ReturnsAsync((AppUser)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(dto.Login)).ReturnsAsync((AppUser)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(dto));
        Assert.Equal("Invalid login attempt.", exception.Message);
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(It.IsAny<AppUser>(), dto.Password, false), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenPasswordIsInvalid()
    {
        // Arrange
        var dto = new LoginUserDto { Login = "testuser", Password = "WrongPassword" };
        var appUser = new AppUser { UserName = dto.Login };

        _mockUserManager.Setup(x => x.FindByEmailAsync(dto.Login)).ReturnsAsync((AppUser)null);
        _mockUserManager.Setup(x => x.FindByNameAsync(dto.Login)).ReturnsAsync(appUser);
        _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(appUser, dto.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.LoginAsync(dto));
        Assert.Equal("Invalid login attempt.", exception.Message);
        _mockSignInManager.Verify(x => x.CheckPasswordSignInAsync(appUser, dto.Password, false), Times.Once);
    }
}
