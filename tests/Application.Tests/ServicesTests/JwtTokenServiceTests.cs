using Application.Services;
using Infrastructure.Data.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace Application.Tests.ServicesTests;

public class JwtTokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _jwtTokenService = new JwtTokenService(_mockConfig.Object);

        // Mocking configuration values for token generation
        _mockConfig.Setup(x => x["Token:Key"]).Returns("68d32736-3129-47a1-a596-bfddd583b383");
        _mockConfig.Setup(x => x["Token:Issuer"]).Returns("test-issuer");
    }

    [Fact]
    public void GenerateToken_ShouldReturnValidJwtToken()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Id = Guid.NewGuid()
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        Assert.NotNull(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("test-issuer", jwtToken.Issuer);
        Assert.Contains(jwtToken.Claims, claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" && claim.Value == user.UserName);
        Assert.Contains(jwtToken.Claims, claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && claim.Value == user.Id.ToString());
    }

    [Fact]
    public void GenerateToken_ShouldIncludeExpiration()
    {
        // Arrange
        var user = new AppUser
        {
            UserName = "testuser",
            Id = Guid.NewGuid()
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
    }
}
