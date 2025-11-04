using AutoMapper;
using Lafise.API;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Services;

public class AccountServiceTests : IDisposable
{
    private readonly BankDataContext _context;
    private readonly AccountSettings _settings;
    private readonly IMapper _mapper;
    private readonly AccountService _accountService;
    private readonly IDbContextFactory<BankDataContext> _dbContextFactory;
    private readonly string _databaseName;

    public AccountServiceTests()
    {
        _databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<BankDataContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new BankDataContext(options);
        
        _settings = new AccountSettings
        {
            ValidAccountTypes = new[] { "Savings", "Checking", "Business" }
        };


        var config = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile<AutoMapperProfile>();
        }, NullLoggerFactory.Instance);
       
        _mapper = config.CreateMapper();

        _dbContextFactory = new TestDbContextFactory(_databaseName);
        _accountService = new AccountService(_dbContextFactory, _settings, _mapper);
    }

    [Fact]
    public async Task CreateAccount_WithValidData_CreatesAccount()
    {
        // Arrange
        var client = new Client
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
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.CreateAccount("client-1", "Savings");

        // Assert
        result.Should().NotBeNull();
        result.ClientId.Should().Be("client-1");
        result.AccountType.Should().Be("Savings");
        result.CurrentBalance.Should().Be(0m);
        result.AccountNumber.Should().NotBeNullOrEmpty();
        result.AccountNumber.Length.Should().Be(7);

        // Verificar en la base de datos usando un nuevo contexto
        using var verifyContext = await _dbContextFactory.CreateDbContextAsync();
        var accountInDb = await verifyContext.Accounts.FirstOrDefaultAsync(a => a.Id == result.Id);
        accountInDb.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateAccount_WithInvalidAccountType_ThrowsException()
    {
        // Arrange
        var client = new Client
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
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _accountService.CreateAccount("client-1", "InvalidType"));
        exception.Message.Should().Contain("Invalid account type");
    }

    [Fact]
    public async Task CreateAccount_WithNonExistentClient_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _accountService.CreateAccount("non-existent-client", "Savings"));
        exception.Message.Should().Contain("No client found");
    }

    [Fact]
    public async Task CreateAccount_WithDuplicateAccountType_ThrowsException()
    {
        // Arrange
        var client = new Client
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
        };

        _context.Clients.Add(client);

        var existingAccount = new Account
        {
            Id = "account-1",
            AccountNumber = "1000000",
            AccountType = "Savings",
            ClientId = "client-1",
            CurrentBalance = 0m
        };

        _context.Accounts.Add(existingAccount);
        await _context.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _accountService.CreateAccount("client-1", "Savings"));
        exception.Message.Should().Contain("already has an account of type");
    }

    [Fact]
    public async Task GetAllAccounts_ReturnsAllAccounts()
    {
        // Arrange
        var client = new Client
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
        };

        var account1 = new Account
        {
            Id = "account-1",
            AccountNumber = "1000000",
            AccountType = "Savings",
            ClientId = "client-1",
            CurrentBalance = 1000m,
            Client = client
        };

        var account2 = new Account
        {
            Id = "account-2",
            AccountNumber = "1000001",
            AccountType = "Checking",
            ClientId = "client-1",
            CurrentBalance = 500m,
            Client = client
        };

        _context.Clients.Add(client);
        _context.Accounts.AddRange(account1, account2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.GetAllAccounts();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Select(a => a.AccountNumber).Should().Contain(new[] { "1000000", "1000001" });
    }

    [Fact]
    public async Task GetAccountDetailsByAccountNumber_WithValidNumber_ReturnsAccount()
    {
        // Arrange
        var client = new Client
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
        };

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = "1000000",
            AccountType = "Savings",
            ClientId = "client-1",
            CurrentBalance = 1000m,
            Client = client
        };

        _context.Clients.Add(client);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.GetAccountDetailsByAccountNumber("1000000");

        // Assert
        result.Should().NotBeNull();
        result.AccountNumber.Should().Be("1000000");
        result.AccountType.Should().Be("Savings");
        result.CurrentBalance.Should().Be(1000m);
        result.Owner.Should().NotBeNull();
        result.Owner.Name.Should().Be("John");
    }

    [Fact]
    public async Task GetAccountDetailsByAccountNumber_WithInvalidNumber_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _accountService.GetAccountDetailsByAccountNumber("9999999"));
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task CreateAccount_GeneratesSequentialAccountNumbers()
    {
        // Arrange
        var client = new Client
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
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        // Act
        var account1 = await _accountService.CreateAccount("client-1", "Savings");
        var account2 = await _accountService.CreateAccount("client-1", "Checking");

        // Assert
        account1.AccountNumber.Should().NotBe(account2.AccountNumber);
        long.Parse(account2.AccountNumber).Should().BeGreaterThan(long.Parse(account1.AccountNumber));
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

