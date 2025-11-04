using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.controllers.Dto;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Accounts.Factories;
using Lafise.API.services.Accounts.Mappers;
using AccountRepository = Lafise.API.services.Accounts.Repositories;
using Lafise.API.services.Accounts.Services;
using Lafise.API.services.Accounts.Validators;
using Lafise.API.services.Auth;
using Lafise.API.services.Transactions.Dto;
using TransactionSummaryDto = Lafise.API.services.Transactions.Dto.TransactionSummaryDto;
using Lafise.API.services.Transactions.Validators;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Accounts
{
    /// <summary>
    /// Service for managing accounts following SOLID principles.
    /// </summary>
    public class AccountService : IAccountService
    {
        private readonly IDbContextFactory<BankDataContext> _db;
        private readonly AccountSettings _settings;
        private readonly IMapper _mapper;
        private readonly IAuthInfo _authInfo;
        private readonly IAccountCreationValidator _accountCreationValidator;
        private readonly AccountRepository.IAccountRepository _accountRepository;
        private readonly IAccountFactory _accountFactory;
        private readonly IAccountNumberGenerator _accountNumberGenerator;
        private readonly IAccountBalanceMapper _accountBalanceMapper;
        private readonly IAccountValidator _accountValidator;

        public AccountService(
            IDbContextFactory<BankDataContext> db,
            AccountSettings settings,
            IMapper mapper,
            IAuthInfo authInfo,
            IAccountCreationValidator accountCreationValidator,
            AccountRepository.IAccountRepository accountRepository,
            IAccountFactory accountFactory,
            IAccountNumberGenerator accountNumberGenerator,
            IAccountBalanceMapper accountBalanceMapper,
            IAccountValidator accountValidator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authInfo = authInfo ?? throw new ArgumentNullException(nameof(authInfo));
            _accountCreationValidator = accountCreationValidator ?? throw new ArgumentNullException(nameof(accountCreationValidator));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _accountFactory = accountFactory ?? throw new ArgumentNullException(nameof(accountFactory));
            _accountNumberGenerator = accountNumberGenerator ?? throw new ArgumentNullException(nameof(accountNumberGenerator));
            _accountBalanceMapper = accountBalanceMapper ?? throw new ArgumentNullException(nameof(accountBalanceMapper));
            _accountValidator = accountValidator ?? throw new ArgumentNullException(nameof(accountValidator));
        }

        public async Task<AccountBalanceDto> GetAccountBalance()
        {
            var clientId = _authInfo.UserId();
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(401, "User not authenticated.");

            var account = await _accountRepository.GetAccountByClientIdAsync(clientId);
            
            if (account == null)
                throw new LafiseException(404, "No account found for the current user.");
            
            return _accountBalanceMapper.MapToBalanceDto(account);
        }

        public async Task<AccountBalanceDto> GetAccountBalance(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new LafiseException(400, "Account number cannot be empty.");

            var account = await _accountRepository.GetAccountByNumberAsync(accountNumber);
            _accountValidator.ValidateAccountExists(account, accountNumber);
            
            if (account == null) throw new InvalidOperationException("Account should not be null after validation");
            return _accountBalanceMapper.MapToBalanceDto(account);
        }

        public async Task<AccountDto> CreateAccount(string accountType)
        {
            var clientId = _authInfo.UserId();
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(401, "User not authenticated.");
            
            return await CreateAccount(clientId, accountType);
        }

        public async Task<AccountDto> CreateAccount(string clientId, string accountType)
        {
            return await CreateAccount(clientId, accountType, null);
        }

        public async Task<AccountDto> CreateAccount(string clientId, string accountType, BankDataContext context)
        {
            var validTypes = _settings.ValidAccountTypes ?? Array.Empty<string>();
            var typeNormalized = _accountCreationValidator.ValidateAndNormalizeAccountType(accountType, validTypes);

            if (context != null)
            {
                // Usar el contexto proporcionado (para transacciones)
                await _accountCreationValidator.ValidateClientExistsAsync(clientId, context);
                await _accountCreationValidator.ValidateNoDuplicateAccountTypeAsync(clientId, typeNormalized, validTypes, context);

                var accountNumber = await _accountNumberGenerator.GenerateAccountNumberAsync();
                var account = _accountFactory.CreateAccount(accountNumber, typeNormalized, clientId);

                context.AddOrUpdate(account);
                await context.SaveChangesAsync();

                return _mapper.Map<AccountDto>(account);
            }
            else
            {
                // Usar contextos nuevos (comportamiento normal)
                await _accountCreationValidator.ValidateClientExistsAsync(clientId);
                await _accountCreationValidator.ValidateNoDuplicateAccountTypeAsync(clientId, typeNormalized, validTypes);

                var accountNumber = await _accountNumberGenerator.GenerateAccountNumberAsync();
                var account = _accountFactory.CreateAccount(accountNumber, typeNormalized, clientId);

                await _accountRepository.SaveAccountAsync(account);

                return _mapper.Map<AccountDto>(account);
            }
        }

    

        public async Task<AccountDto> GetAccountDetailsByAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new LafiseException(400, "Account number cannot be empty.");

            var account = await _accountRepository.GetAccountByNumberAsync(accountNumber);
            _accountValidator.ValidateAccountExists(account, accountNumber);

            if (account == null) throw new InvalidOperationException("Account should not be null after validation");
            return _mapper.Map<AccountDto>(account);
        }

        public async Task<PagedDtoSummary<TransactionDto, TransactionSummaryDto>> GetAccountMovements(string accountNumber, PaginationRequestDto pagination)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new LafiseException(400, "Account number cannot be empty.");

            // Validar parámetros de paginación
            if (pagination.Page < 1)
                throw new LafiseException(400, "Page number must be greater than 0.");
            if (pagination.PageSize < 1 || pagination.PageSize > 100)
                throw new LafiseException(400, "Page size must be between 1 and 100.");

            // Obtener el ID del usuario autenticado
            var clientId = _authInfo.UserId();
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(401, "User not authenticated.");

            var account = await _accountRepository.GetAccountByNumberAsync(accountNumber);
            _accountValidator.ValidateAccountExists(account, accountNumber);
            
            if (account == null) throw new InvalidOperationException("Account should not be null after validation");

            // Validar que la cuenta pertenezca al usuario autenticado
            if (account.ClientId != clientId)
            {
                throw new LafiseException(403, 
                    "You cannot view movements from other accounts. You can only view movements from your own accounts.");
            }

            using var context = await _db.CreateDbContextAsync();

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

            // Calcular resumen de todas las transacciones (no solo las paginadas)
            var allTransactions = await query.ToListAsync();
            
            var summary = new TransactionSummaryDto
            {
                TotalDeposits = allTransactions.Count(t => t.Type.Equals("Deposit", StringComparison.OrdinalIgnoreCase)),
                TotalDepositsAmount = allTransactions
                    .Where(t => t.Type.Equals("Deposit", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                TotalWithdrawals = allTransactions.Count(t => t.Type.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase)),
                TotalWithdrawalsAmount = allTransactions
                    .Where(t => t.Type.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                TotalTransfersOut = allTransactions.Count(t => t.Type.Equals("Transfer Out", StringComparison.OrdinalIgnoreCase)),
                TotalTransfersOutAmount = allTransactions
                    .Where(t => t.Type.Equals("Transfer Out", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                TotalTransfersIn = allTransactions.Count(t => t.Type.Equals("Transfer In", StringComparison.OrdinalIgnoreCase)),
                TotalTransfersInAmount = allTransactions
                    .Where(t => t.Type.Equals("Transfer In", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                CurrentBalance = account.CurrentBalance
            };

            // Calcular monto neto
            summary.NetAmount = (summary.TotalDepositsAmount + summary.TotalTransfersInAmount) 
                - (summary.TotalWithdrawalsAmount + summary.TotalTransfersOutAmount);

            var transactionDtos = _mapper.Map<List<TransactionDto>>(pagedResults);
            foreach (var dto in transactionDtos)
            {
                dto.AccountNumber = account.AccountNumber;
            }

            var response = new PagedDtoSummary<TransactionDto, TransactionSummaryDto>
            {
                Data = transactionDtos,
                Pagination = new Pagination
                {
                    TotalCount = totalCount,
                    Count = transactionDtos.Count,
                    CurrentPage = pagination.Page,
                    TotalPages = totalPages,
                    PageSize = pagination.PageSize
                },
                Summary = summary
            };

            return response;
        }
    }
}
