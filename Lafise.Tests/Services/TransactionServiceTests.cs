using AutoMapper;
using Lafise.API;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Auth;
using Lafise.API.services.Transactions;
using Lafise.API.services.Transactions.Dto;
using Lafise.API.services.Transactions.Factories;
using Lafise.API.services.Transactions.Repositories;
using Lafise.API.services.Transactions.Validators;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using FluentAssertions;
using Xunit;

namespace Lafise.Tests.Services;

public class TransactionServiceTests : IDisposable
{
    private readonly BankDataContext _context;
    private readonly IDbContextFactory<BankDataContext> _dbContextFactory;
    private readonly IMapper _mapper;
    private readonly Mock<IAuthInfo> _mockAuthInfo;
    private readonly Mock<ITransactionValidator> _mockTransactionValidator;
    private readonly Mock<IAccountValidator> _mockAccountValidator;
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly ITransactionFactory _transactionFactory;
    private readonly TransactionService _transactionService;
    private readonly string _databaseName;

    public TransactionServiceTests()
    {
        _databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<BankDataContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _context = new BankDataContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }, NullLoggerFactory.Instance);

        _mapper = config.CreateMapper();

        _mockAuthInfo = new Mock<IAuthInfo>();
        _mockTransactionValidator = new Mock<ITransactionValidator>();
        _mockAccountValidator = new Mock<IAccountValidator>();
        _mockAccountRepository = new Mock<IAccountRepository>();
        _transactionFactory = new TransactionFactory();

        _dbContextFactory = new TestDbContextFactory(_databaseName);

        _transactionService = new TransactionService(
            _dbContextFactory,
            _mapper,
            _mockAuthInfo.Object,
            _mockTransactionValidator.Object,
            _mockAccountValidator.Object,
            _mockAccountRepository.Object,
            _transactionFactory);
    }

    #region Deposit Tests

    [Fact]
    public async Task Deposit_WithValidRequest_CreatesTransactionAndUpdatesBalance()
    {
        // Arrange
        var clientId = "client-1";
        var accountNumber = "1000000";
        var initialBalance = 1000m;
        var depositAmount = 500m;
        var expectedBalance = initialBalance + depositAmount;

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = accountNumber,
            AccountType = "Savings",
            ClientId = clientId,
            CurrentBalance = initialBalance
        };

        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = depositAmount
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(account, accountNumber));
        _mockAccountValidator.Setup(x => x.ValidateAccountOwnership(account, clientId, "deposit"))
            .Returns(Task.CompletedTask);

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.Deposit(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountNumber.Should().Be(accountNumber);
        result.Type.Should().Be("Deposit");
        result.Amount.Should().Be(depositAmount);
        result.BalanceAfter.Should().Be(expectedBalance);

        // Verificar que el saldo se actualizó en la base de datos
        using var verifyContext = await _dbContextFactory.CreateDbContextAsync();
        var updatedAccount = await verifyContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.CurrentBalance.Should().Be(expectedBalance);

        // Verificar que la transacción se guardó
        var transaction = await verifyContext.Transactions
            .FirstOrDefaultAsync(t => t.AccountId == account.Id && t.Type == "Deposit");
        transaction.Should().NotBeNull();
        transaction!.Amount.Should().Be(depositAmount);
        transaction.BalanceAfter.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task Deposit_WithInvalidAccount_ThrowsException()
    {
        // Arrange
        var clientId = "client-1";
        var accountNumber = "9999999";
        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = 500m
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync((Account)null!);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(null, accountNumber))
            .Throws(new LafiseException(404, "Account not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _transactionService.Deposit(request));
        exception.Code.Should().Be(404);
        exception.Message.Should().Contain("not found");
    }

    [Fact]
    public async Task Deposit_WithUnauthorizedUser_ThrowsException()
    {
        // Arrange
        var clientId = "client-1";
        var otherClientId = "client-2";
        var accountNumber = "1000000";
        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = 500m
        };

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = accountNumber,
            AccountType = "Savings",
            ClientId = otherClientId,
            CurrentBalance = 1000m
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(account, accountNumber));
        _mockAccountValidator.Setup(x => x.ValidateAccountOwnership(account, clientId, "deposit"))
            .ThrowsAsync(new LafiseException(403, "You cannot deposit money to other accounts"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _transactionService.Deposit(request));
        exception.Code.Should().Be(403);
    }

    #endregion

    #region Withdrawal Tests

    [Fact]
    public async Task Withdraw_WithValidRequest_CreatesTransactionAndUpdatesBalance()
    {
        // Arrange
        var clientId = "client-1";
        var accountNumber = "1000000";
        var initialBalance = 1000m;
        var withdrawalAmount = 300m;
        var expectedBalance = initialBalance - withdrawalAmount;

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = accountNumber,
            AccountType = "Savings",
            ClientId = clientId,
            CurrentBalance = initialBalance
        };

        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = withdrawalAmount
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(account, accountNumber));
        _mockAccountValidator.Setup(x => x.ValidateAccountOwnership(account, clientId, "withdraw"))
            .Returns(Task.CompletedTask);
        _mockAccountValidator.Setup(x => x.ValidateSufficientBalance(account, withdrawalAmount));

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.Withdraw(request);

        // Assert
        result.Should().NotBeNull();
        result.AccountNumber.Should().Be(accountNumber);
        result.Type.Should().Be("Withdrawal");
        result.Amount.Should().Be(withdrawalAmount);
        result.BalanceAfter.Should().Be(expectedBalance);

        // Verificar que el saldo se actualizó
        using var verifyContext = await _dbContextFactory.CreateDbContextAsync();
        var updatedAccount = await verifyContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
        updatedAccount.Should().NotBeNull();
        updatedAccount!.CurrentBalance.Should().Be(expectedBalance);

        // Verificar que la transacción se guardó
        var transaction = await verifyContext.Transactions
            .FirstOrDefaultAsync(t => t.AccountId == account.Id && t.Type == "Withdrawal");
        transaction.Should().NotBeNull();
        transaction!.Amount.Should().Be(withdrawalAmount);
        transaction.BalanceAfter.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task Withdraw_WithInsufficientFunds_ThrowsException()
    {
        // Arrange
        var clientId = "client-1";
        var accountNumber = "1000000";
        var initialBalance = 100m;
        var withdrawalAmount = 500m;

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = accountNumber,
            ClientId = clientId,
            CurrentBalance = initialBalance,
            AccountType = "Savings"



        };

        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = withdrawalAmount
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(account, accountNumber));
        _mockAccountValidator.Setup(x => x.ValidateAccountOwnership(account, clientId, "withdraw"))
            .Returns(Task.CompletedTask);
        _mockAccountValidator.Setup(x => x.ValidateSufficientBalance(account, withdrawalAmount))
            .Throws(new LafiseException(400, "Insufficient funds"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _transactionService.Withdraw(request));
        exception.Code.Should().Be(400);
        exception.Message.Should().Contain("Insufficient funds");
    }

    [Fact]
    public async Task Withdraw_WithInvalidAccount_ThrowsException()
    {
        // Arrange
        var clientId = "client-1";
        var accountNumber = "9999999";
        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = 500m
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync((Account)null!);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(null, accountNumber))
            .Throws(new LafiseException(404, "Account not found"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _transactionService.Withdraw(request));
        exception.Code.Should().Be(404);
    }

    [Fact]
    public async Task Withdraw_WithUnauthorizedUser_ThrowsException()
    {
        // Arrange
        var clientId = "client-1";
        var otherClientId = "client-2";
        var accountNumber = "1000000";
        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = 500m
        };

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = accountNumber,
            AccountType = "Savings",
            ClientId = otherClientId,
            CurrentBalance = 1000m
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(account, accountNumber));
        _mockAccountValidator.Setup(x => x.ValidateAccountOwnership(account, clientId, "withdraw"))
            .ThrowsAsync(new LafiseException(403, "You cannot withdraw money from other accounts"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LafiseException>(() => 
            _transactionService.Withdraw(request));
        exception.Code.Should().Be(403);
    }

    [Fact]
    public async Task Withdraw_WithExactBalance_AllowsTransaction()
    {
        // Arrange
        var clientId = "client-1";
        var accountNumber = "1000000";
        var initialBalance = 500m;
        var withdrawalAmount = 500m;
        var expectedBalance = 0m;

        var account = new Account
        {
            Id = "account-1",
            AccountNumber = accountNumber,
            AccountType = "Savings",
            ClientId = clientId,
            CurrentBalance = initialBalance
        };

        var request = new CreateTransactionDto
        {
            AccountNumber = accountNumber,
            Amount = withdrawalAmount
        };

        _mockAuthInfo.Setup(x => x.UserId()).Returns(clientId);
        _mockTransactionValidator.Setup(x => x.ValidateTransactionRequest(request));
        _mockAccountRepository.Setup(x => x.GetAccountByNumberAsync(accountNumber))
            .ReturnsAsync(account);
        _mockAccountValidator.Setup(x => x.ValidateAccountExists(account, accountNumber));
        _mockAccountValidator.Setup(x => x.ValidateAccountOwnership(account, clientId, "withdraw"))
            .Returns(Task.CompletedTask);
        _mockAccountValidator.Setup(x => x.ValidateSufficientBalance(account, withdrawalAmount));

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Act
        var result = await _transactionService.Withdraw(request);

        // Assert
        result.Should().NotBeNull();
        result.BalanceAfter.Should().Be(expectedBalance);

        using var verifyContext = await _dbContextFactory.CreateDbContextAsync();
        var updatedAccount = await verifyContext.Accounts.FirstOrDefaultAsync(a => a.Id == account.Id);
        updatedAccount!.CurrentBalance.Should().Be(expectedBalance);
    }

    #endregion

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
            return new BankDataContext(_options);
        }

        public async Task<BankDataContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(new BankDataContext(_options));
        }
    }
}

