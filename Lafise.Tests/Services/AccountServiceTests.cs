using AutoMapper;
using Lafise.API;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Accounts.Factories;
using Lafise.API.services.Accounts.Mappers;
using Lafise.API.services.Accounts.Repositories;
using Lafise.API.services.Accounts.Services;
using Lafise.API.services.Accounts.Validators;
using Lafise.API.services.Auth;
using Lafise.API.services.Transactions.Validators;
using Lafise.API.controllers.Dto;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
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
    private readonly Mock<IAuthInfo> _mockAuthInfo;
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

        _mockAuthInfo = new Mock<IAuthInfo>();
        _dbContextFactory = new TestDbContextFactory(_databaseName);

        // Crear instancias reales de las dependencias
        var accountRepository = new AccountRepository(_dbContextFactory);
        var accountFactory = new AccountFactory();
        var accountNumberGenerator = new AccountNumberGenerator(accountRepository);
        var accountBalanceMapper = new AccountBalanceMapper();
        var accountCreationValidator = new AccountCreationValidator(_dbContextFactory);
        var accountValidator = new AccountValidator();

        _accountService = new AccountService(
            _dbContextFactory,
            _settings,
            _mapper,
            _mockAuthInfo.Object,
            accountCreationValidator,
            accountRepository,
            accountFactory,
            accountNumberGenerator,
            accountBalanceMapper,
            accountValidator);
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
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _accountService.CreateAccount("client-1", "InvalidType"));
        exception.Code.Should().Be(400);
        exception.Message.Should().Contain("Invalid account type");
    }

    [Fact]
    public async Task CreateAccount_WithNonExistentClient_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _accountService.CreateAccount("non-existent-client", "Savings"));
        exception.Code.Should().Be(404);
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
        var exception = await Assert.ThrowsAsync<LafiseException>(() => _accountService.CreateAccount("client-1", "Savings"));
        exception.Code.Should().Be(400);
        exception.Message.Should().Contain("already have");
    }

    #region GetAccountBalance Tests

    [Fact]
    public async Task GetAccountBalance_WithValidAccountNumber_ReturnsBalance()
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
            CurrentBalance = 2500.50m
        };

        _context.Clients.Add(client);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _accountService.GetAccountBalance("1000000");

        // Assert
        result.Should().NotBeNull();
        result.AccountNumber.Should().Be("1000000");
        result.AccountType.Should().Be("Savings");
        result.CurrentBalance.Should().Be(2500.50m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public async Task GetAccountBalance_WithInvalidAccountNumber_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _accountService.GetAccountBalance("9999999"));
        exception.Code.Should().Be(404);
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task GetAccountBalance_WithEmptyAccountNumber_ThrowsException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _accountService.GetAccountBalance(""));
        exception.Code.Should().Be(400);
        exception.Message.Should().Contain("cannot be empty");
    }

    #endregion

    #region GetAccountMovements Tests

    [Fact]
    public async Task GetAccountMovements_WithValidAccount_ReturnsTransactionHistory()
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
            CurrentBalance = 1200m
        };

        var transactions = new List<Transaction>
        {
            new Transaction
            {
                Id = "trans-1",
                AccountId = "account-1",
                Type = "Deposit",
                Amount = 1000m,
                BalanceAfter = 1000m,
                Date = DateTime.UtcNow.AddDays(-2)
            },
            new Transaction
            {
                Id = "trans-2",
                AccountId = "account-1",
                Type = "Deposit",
                Amount = 500m,
                BalanceAfter = 1500m,
                Date = DateTime.UtcNow.AddDays(-1)
            },
            new Transaction
            {
                Id = "trans-3",
                AccountId = "account-1",
                Type = "Withdrawal",
                Amount = 300m,
                BalanceAfter = 1200m,
                Date = DateTime.UtcNow
            }
        };

        _context.Clients.Add(client);
        _context.Accounts.Add(account);
        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync();

        _mockAuthInfo.Setup(x => x.UserId()).Returns("client-1");

        var pagination = new PaginationRequestDto { Page = 1, PageSize = 10 };

        // Act
        var result = await _accountService.GetAccountMovements("1000000", pagination);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.Summary.Should().NotBeNull();
        result.Summary.TotalDeposits.Should().Be(2);
        result.Summary.TotalDepositsAmount.Should().Be(1500m);
        result.Summary.TotalWithdrawals.Should().Be(1);
        result.Summary.TotalWithdrawalsAmount.Should().Be(300m);
        result.Summary.CurrentBalance.Should().Be(1200m);
        result.Summary.NetAmount.Should().Be(1200m);
        
        // Verificar que las transacciones están ordenadas por fecha descendente
        result.Data[0].Date.Should().BeAfter(result.Data[1].Date);
    }

    [Fact]
    public async Task GetAccountMovements_WithPagination_ReturnsPaginatedResults()
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
            CurrentBalance = 5000m
        };

        var transactions = new List<Transaction>();
        for (int i = 0; i < 15; i++)
        {
            transactions.Add(new Transaction
            {
                Id = $"trans-{i}",
                AccountId = "account-1",
                Type = i % 2 == 0 ? "Deposit" : "Withdrawal",
                Amount = 100m,
                BalanceAfter = 5000m,
                Date = DateTime.UtcNow.AddDays(-i)
            });
        }

        _context.Clients.Add(client);
        _context.Accounts.Add(account);
        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync();

        _mockAuthInfo.Setup(x => x.UserId()).Returns("client-1");

        var pagination = new PaginationRequestDto { Page = 1, PageSize = 5 };

        // Act
        var result = await _accountService.GetAccountMovements("1000000", pagination);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(5);
        result.Pagination.Should().NotBeNull();
        result.Pagination.TotalCount.Should().Be(15);
        result.Pagination.CurrentPage.Should().Be(1);
        result.Pagination.PageSize.Should().Be(5);
        result.Pagination.TotalPages.Should().Be(3);
    }

    [Fact]
    public async Task GetAccountMovements_WithInvalidAccount_ThrowsException()
    {
        // Arrange
        _mockAuthInfo.Setup(x => x.UserId()).Returns("client-1");
        var pagination = new PaginationRequestDto { Page = 1, PageSize = 10 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _accountService.GetAccountMovements("9999999", pagination));
        exception.Code.Should().Be(404);
    }

    [Fact]
    public async Task GetAccountMovements_WithUnauthorizedUser_ThrowsException()
    {
        // Arrange
        var client1 = new Client
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

        var client2 = new Client
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
        };

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = "1000000",
            AccountType = "Savings",
            ClientId = "client-2", // Cuenta pertenece a client-2
            CurrentBalance = 1000m
        };

        _context.Clients.AddRange(client1, client2);
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        _mockAuthInfo.Setup(x => x.UserId()).Returns("client-1"); // Usuario autenticado es client-1
        var pagination = new PaginationRequestDto { Page = 1, PageSize = 10 };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _accountService.GetAccountMovements("1000000", pagination));
        exception.Code.Should().Be(403);
    }

    #endregion

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

