using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Auth;
using Lafise.API.services.Auth.Dto;
using Lafise.API.services.Auth.JWT;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly BankDataContext _context;
    private readonly Mock<ICryptor> _mockCryptor;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly AuthService _authService;
    private readonly IDbContextFactory<BankDataContext> _dbContextFactory;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<BankDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new BankDataContext(options);
        _mockCryptor = new Mock<ICryptor>();
        _mockConfig = new Mock<IConfiguration>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();

        _dbContextFactory = new TestDbContextFactory(_context);
        _authService = new AuthService(_dbContextFactory, _mockCryptor.Object, _mockConfig.Object, _mockJwtTokenGenerator.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsLoginResult()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Test1234!";
        var passwordHash = "hash123";
        var passwordSalt = "salt123";

        var client = new Client
        {
            Id = "client-1",
            Name = "John",
            LastName = "Doe",
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            TaxId = "123456789",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "M",
            Income = 50000m
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var tokenInfo = new TokenInfo
        {
            Token = "jwt-token-123",
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        _mockCryptor
            .Setup(x => x.VerifyPasswordHash(password, passwordHash, passwordSalt))
            .Returns(true);

        _mockJwtTokenGenerator
            .Setup(x => x.CreateToken(It.IsAny<Client>()))
            .Returns(tokenInfo);

        _mockJwtTokenGenerator
            .Setup(x => x.GenerateRefreshToken(It.IsAny<Client>()))
            .Returns("refresh-token-123");

        // Act
        var result = await _authService.Login(email, password);

        // Assert
        result.Should().NotBeNull();
        result.UserEmail.Should().Be(email);
        result.UserName.Should().Be("John Doe");
        result.AuthToken.Should().Be("jwt-token-123");
        result.RefreshToken.Should().Be("refresh-token-123");

        _mockCryptor.Verify(x => x.VerifyPasswordHash(password, passwordHash, passwordSalt), Times.Once);
        _mockJwtTokenGenerator.Verify(x => x.CreateToken(It.IsAny<Client>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ThrowsLafiseException()
    {
        // Arrange
        var email = "";
        var password = "Test1234!";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _authService.Login(email, password));
        exception.Code.Should().Be(400);
        exception.Message.Should().Contain("Email cannot be empty");
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ThrowsLafiseException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _authService.Login(email, password));
        exception.Code.Should().Be(400);
        exception.Message.Should().Contain("Password cannot be empty");
    }

    [Fact]
    public async Task Login_WithUserNotFound_ThrowsLafiseException()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "Test1234!";

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _authService.Login(email, password));
        exception.Code.Should().Be(404);
        exception.Message.Should().Contain("User not found");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ThrowsLafiseException()
    {
        // Arrange
        var email = "test@example.com";
        var password = "WrongPassword123!";
        var passwordHash = "hash123";
        var passwordSalt = "salt123";

        var client = new Client
        {
            Id = "client-1",
            Name = "John",
            LastName = "Doe",
            Email = email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            TaxId = "123456789",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "M",
            Income = 50000m
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        _mockCryptor
            .Setup(x => x.VerifyPasswordHash(password, passwordHash, passwordSalt))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _authService.Login(email, password));
        exception.Code.Should().Be(401);
        exception.Message.Should().Contain("Invalid password");
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    // Helper class to create DbContextFactory for testing
    private class TestDbContextFactory : IDbContextFactory<BankDataContext>
    {
        private readonly BankDataContext _context;

        public TestDbContextFactory(BankDataContext context)
        {
            _context = context;
        }

        public BankDataContext CreateDbContext()
        {
            return _context;
        }

        public async Task<BankDataContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(_context);
        }
    }
}

