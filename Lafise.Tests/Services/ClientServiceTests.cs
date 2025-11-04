using AutoMapper;
using Lafise.API;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts;
using Lafise.API.services.clients;
using Lafise.API.services.Clients.Dto;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Services;

public class ClientServiceTests : IDisposable
{
    private readonly BankDataContext _context;
    private readonly Mock<IAccountService> _mockAccountService;
    private readonly Mock<ICryptor> _mockCryptor;
    private readonly IMapper _mapper;
    private readonly ClientService _clientService;
    private readonly IDbContextFactory<BankDataContext> _dbContextFactory;
    private readonly string _databaseName;

    public ClientServiceTests()
    {
        _databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<BankDataContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new BankDataContext(options);
        _mockAccountService = new Mock<IAccountService>();
        _mockCryptor = new Mock<ICryptor>();

        // Configure AutoMapper (AutoMapper 15.x requires ILoggerFactory)
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }, NullLoggerFactory.Instance);
        // No validar configuración estricta en tests (el perfil ya tiene los mapeos configurados)
        _mapper = config.CreateMapper();

        _dbContextFactory = new TestDbContextFactory(_databaseName);
        _clientService = new ClientService(_dbContextFactory, _mockAccountService.Object, _mockCryptor.Object, _mapper);
    }

    [Fact]
    public async Task GetAllClients_ReturnsAllClients()
    {
        // Arrange
        var clients = new List<Client>
        {
            new Client
            {
                Id = "client-1",
                Name = "John",
                LastName = "Doe",
                Email = "john@example.com",
                TaxId = "123456789",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                Gender = "M",
                Income = 50000m,
                PasswordHash = "test-hash",
                PasswordSalt = "test-salt"
            },
            new Client
            {
                Id = "client-2",
                Name = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                TaxId = "987654321",
                DateOfBirth = DateTime.UtcNow.AddYears(-25),
                Gender = "F",
                Income = 60000m,
                PasswordHash = "test-hash-2",
                PasswordSalt = "test-salt-2"
            }
        };

        _context.Clients.AddRange(clients);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clientService.GetAllClients();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(c => c.Id).Should().Contain(new[] { "client-1", "client-2" });
    }

    [Fact]
    public async Task CreateClient_WithValidData_CreatesClient()
    {
        // Arrange
        var createClientDto = new CreateClientDto
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            TaxId = "123456789",
            Password = "Test1234!",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "M",
            Income = 50000m,
            AccountType = "Savings"
        };

        _mockCryptor
            .Setup(x => x.ValidatePassword(createClientDto.Password))
            .Verifiable();

        _mockCryptor
            .Setup(x => x.CreatePasswordHash(It.IsAny<string>(), It.IsAny<Client>()))
            .Callback<string, Client>((password, client) =>
            {
                client.PasswordHash = "hashed-password";
                client.PasswordSalt = "salt";
            });

        _mockAccountService
            .Setup(x => x.CreateAccount(It.IsAny<BankDataContext>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new Account
            {
                Id = "account-1",
                AccountNumber = "1000000",
                AccountType = "Savings",
                ClientId = "client-id",
                CurrentBalance = 0m
            });

        // Act
        var result = await _clientService.CreateClient(createClientDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.Email.Should().Be("john@example.com");

        // Verificar en la base de datos usando un nuevo contexto
        using var verifyContext = await _dbContextFactory.CreateDbContextAsync();
        var clientInDb = await verifyContext.Clients.FirstOrDefaultAsync(c => c.TaxId == "123456789");
        clientInDb.Should().NotBeNull();
        clientInDb!.Name.Should().Be("John");

        _mockCryptor.Verify(x => x.ValidatePassword(createClientDto.Password), Times.Once);
        _mockAccountService.Verify(x => x.CreateAccount(It.IsAny<BankDataContext>(), It.IsAny<string>(), "Savings"), Times.Once);
    }

    [Fact]
    public async Task CreateClient_WithDuplicateTaxId_ThrowsException()
    {
        // Arrange
        var existingClient = new Client
        {
            Id = "existing-client",
            Name = "Existing",
            LastName = "Client",
            TaxId = "123456789",
            Email = "existing@example.com",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "M",
            Income = 50000m,
            PasswordHash = "existing-hash",
            PasswordSalt = "existing-salt"
        };

        _context.Clients.Add(existingClient);
        await _context.SaveChangesAsync();

        var createClientDto = new CreateClientDto
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            TaxId = "123456789", // Duplicate
            Password = "Test1234!",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "M",
            Income = 50000m,
            AccountType = "Savings"
        };

        _mockCryptor
            .Setup(x => x.ValidatePassword(createClientDto.Password))
            .Verifiable();

        _mockCryptor
            .Setup(x => x.CreatePasswordHash(It.IsAny<string>(), It.IsAny<Client>()))
            .Callback<string, Client>((password, client) =>
            {
                client.PasswordHash = "hashed-password";
                client.PasswordSalt = "salt";
            });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _clientService.CreateClient(createClientDto));
        exception.Message.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateClient_NormalizesTaxId()
    {
        // Arrange
        var createClientDto = new CreateClientDto
        {
            Name = "John",
            LastName = "Doe",
            Email = "john@example.com",
            TaxId = "123-456-789", // With dashes
            Password = "Test1234!",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Gender = "M",
            Income = 50000m,
            AccountType = ""
        };

        _mockCryptor
            .Setup(x => x.ValidatePassword(createClientDto.Password))
            .Verifiable();

        _mockCryptor
            .Setup(x => x.CreatePasswordHash(It.IsAny<string>(), It.IsAny<Client>()))
            .Callback<string, Client>((password, client) =>
            {
                client.PasswordHash = "hashed-password";
                client.PasswordSalt = "salt";
            });

        // Act
        var result = await _clientService.CreateClient(createClientDto);

        // Assert
        // Verificar en la base de datos usando un nuevo contexto
        using var verifyContext = await _dbContextFactory.CreateDbContextAsync();
        var clientInDb = await verifyContext.Clients.FirstOrDefaultAsync(c => c.Email == "john@example.com");
        clientInDb.Should().NotBeNull();
        clientInDb!.TaxId.Should().Be("123456789"); // Should be normalized (no dashes)
    }

    public void Dispose()
    {
        _context?.Dispose();
    }

    private class TestDbContextFactory : IDbContextFactory<BankDataContext>
    {
        private readonly DbContextOptions<BankDataContext> _options;

        public TestDbContextFactory(string databaseName)
        {
            _options = new DbContextOptionsBuilder<BankDataContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        public BankDataContext CreateDbContext()
        {
            // Crear nueva instancia pero compartiendo la misma base de datos en memoria
            return new BankDataContext(_options);
        }

        public async Task<BankDataContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            // Crear nueva instancia pero compartiendo la misma base de datos en memoria
            return await Task.FromResult(new BankDataContext(_options));
        }
    }
}

