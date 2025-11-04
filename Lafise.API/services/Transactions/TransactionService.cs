using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.controllers.Dto;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Auth;
using Lafise.API.services.Transactions.Dto;
using Lafise.API.services.Transactions.Factories;
using Lafise.API.services.Transactions.Repositories;
using Lafise.API.services.Transactions.Validators;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Transactions
{
    /// <summary>
    /// Service for managing transactions following SOLID principles.
    /// </summary>
    public class TransactionService : ITransactionService
    {
        private readonly IDbContextFactory<BankDataContext> _db;
        private readonly IMapper _mapper;
        private readonly IAuthInfo _authInfo;
        private readonly ITransactionValidator _transactionValidator;
        private readonly IAccountValidator _accountValidator;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionFactory _transactionFactory;

        public TransactionService(
            IDbContextFactory<BankDataContext> db,
            IMapper mapper,
            IAuthInfo authInfo,
            ITransactionValidator transactionValidator,
            IAccountValidator accountValidator,
            IAccountRepository accountRepository,
            ITransactionFactory transactionFactory)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authInfo = authInfo ?? throw new ArgumentNullException(nameof(authInfo));
            _transactionValidator = transactionValidator ?? throw new ArgumentNullException(nameof(transactionValidator));
            _accountValidator = accountValidator ?? throw new ArgumentNullException(nameof(accountValidator));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _transactionFactory = transactionFactory ?? throw new ArgumentNullException(nameof(transactionFactory));
        }

        public async Task<TransactionDto> Deposit(CreateTransactionDto request)
        {
            var clientId = GetAuthenticatedClientId();
            _transactionValidator.ValidateTransactionRequest(request);

            var account = await _accountRepository.GetAccountByNumberAsync(request.AccountNumber);
            _accountValidator.ValidateAccountExists(account, request.AccountNumber);
            await _accountValidator.ValidateAccountOwnership(account, clientId, "deposit");

            var newBalance = account.CurrentBalance + request.Amount;
            account.CurrentBalance = newBalance;

            var transaction = _transactionFactory.CreateDepositTransaction(account.Id, request.Amount, newBalance);

            await SaveTransactionAndAccountAsync(transaction, account);

            return MapToDto(transaction, account.AccountNumber);
        }

        public async Task<TransactionDto> Withdraw(CreateTransactionDto request)
        {
            var clientId = GetAuthenticatedClientId();
            _transactionValidator.ValidateTransactionRequest(request);

            var account = await _accountRepository.GetAccountByNumberAsync(request.AccountNumber);
            _accountValidator.ValidateAccountExists(account, request.AccountNumber);
            await _accountValidator.ValidateAccountOwnership(account, clientId, "withdraw");
            _accountValidator.ValidateSufficientBalance(account, request.Amount);

            var newBalance = account.CurrentBalance - request.Amount;
            account.CurrentBalance = newBalance;

            var transaction = _transactionFactory.CreateWithdrawalTransaction(account.Id, request.Amount, newBalance);

            await SaveTransactionAndAccountAsync(transaction, account);

            return MapToDto(transaction, account.AccountNumber);
        }

        public async Task<PagedDto<TransactionDto>> GetAccountMovements(string accountNumber, PaginationRequestDto pagination)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new LafiseException(400, "Account number cannot be empty.");

            // Validar parámetros de paginación
            if (pagination.Page < 1)
                throw new LafiseException(400, "Page number must be greater than 0.");
            if (pagination.PageSize < 1 || pagination.PageSize > 100)
                throw new LafiseException(400, "Page size must be between 1 and 100.");

            using var context = await _db.CreateDbContextAsync();

            var account = await _accountRepository.GetAccountByNumberAsync(accountNumber);
            _accountValidator.ValidateAccountExists(account, accountNumber);

            var query = context.Transactions
                .Where(t => t.AccountId == account.Id);

            // Contar total de elementos
            var totalCount = await query.CountAsync();

            // Proteger contra división por cero
            if (pagination.PageSize == 0)
                pagination.PageSize = 10;

            // Paginación
            var pagedResults = await query
                .OrderByDescending(t => t.Date)
                .Skip((pagination.Page - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            // Calcular total de páginas
            var totalPages = (int)Math.Ceiling((double)totalCount / pagination.PageSize);

            var transactionDtos = MapTransactionsToDto(pagedResults, account.AccountNumber);

            var response = new PagedDto<TransactionDto>
            {
                Data = transactionDtos,
                Pagination = new Pagination
                {
                    TotalCount = totalCount,
                    Count = transactionDtos.Count,
                    CurrentPage = pagination.Page,
                    TotalPages = totalPages,
                    PageSize = pagination.PageSize
                }
            };

            return response;
        }

        public async Task<TransactionDto> Transfer(TransferDto request)
        {
            var clientId = GetAuthenticatedClientId();
            _transactionValidator.ValidateTransferRequest(request);

            var fromAccount = await _accountRepository.GetAccountByNumberAsync(request.FromAccountNumber);
            _accountValidator.ValidateAccountExists(fromAccount, request.FromAccountNumber);
            await _accountValidator.ValidateAccountOwnership(fromAccount, clientId, "transfer");
            _accountValidator.ValidateSufficientBalance(fromAccount, request.Amount);

            var toAccount = await _accountRepository.GetAccountByNumberAsync(request.ToAccountNumber);
            _accountValidator.ValidateAccountExists(toAccount, request.ToAccountNumber);

            var fromNewBalance = fromAccount.CurrentBalance - request.Amount;
            var toNewBalance = toAccount.CurrentBalance + request.Amount;

            fromAccount.CurrentBalance = fromNewBalance;
            toAccount.CurrentBalance = toNewBalance;

            var withdrawalTransaction = _transactionFactory.CreateTransferOutTransaction(
                fromAccount.Id, request.Amount, fromNewBalance);
            var depositTransaction = _transactionFactory.CreateTransferInTransaction(
                toAccount.Id, request.Amount, toNewBalance);

            await SaveTransferAsync(withdrawalTransaction, depositTransaction, fromAccount, toAccount);

            return MapToDto(withdrawalTransaction, fromAccount.AccountNumber);
        }

        private string GetAuthenticatedClientId()
        {
            var clientId = _authInfo.UserId();
            _transactionValidator.ValidateAuthentication(clientId);
            return clientId;
        }

        private async Task SaveTransactionAndAccountAsync(Transaction transaction, Account account)
        {
            using var context = await _db.CreateDbContextAsync();
            context.AddOrUpdate(transaction);
            context.AddOrUpdate(account);
            await context.SaveChangesAsync();
        }

        private async Task SaveTransferAsync(
            Transaction withdrawalTransaction,
            Transaction depositTransaction,
            Account fromAccount,
            Account toAccount)
        {
            using var context = await _db.CreateDbContextAsync();
            context.AddOrUpdate(withdrawalTransaction);
            context.AddOrUpdate(depositTransaction);
            context.AddOrUpdate(fromAccount);
            context.AddOrUpdate(toAccount);
            await context.SaveChangesAsync();
        }

        private TransactionDto MapToDto(Transaction transaction, string accountNumber)
        {
            var transactionDto = _mapper.Map<TransactionDto>(transaction);
            transactionDto.AccountNumber = accountNumber;
            return transactionDto;
        }

        private List<TransactionDto> MapTransactionsToDto(List<Transaction> transactions, string accountNumber)
        {
            var transactionDtos = _mapper.Map<List<TransactionDto>>(transactions);
            foreach (var dto in transactionDtos)
            {
                dto.AccountNumber = accountNumber;
            }
            return transactionDtos;
        }
    }
}
