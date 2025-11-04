using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IDbContextFactory<BankDataContext> _db;
        private readonly AccountSettings _settings;
        private readonly IMapper _mapper;

        public AccountService(IDbContextFactory<BankDataContext> db, AccountSettings settings, IMapper mapper)
        {
            _db = db;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Account> CreateAccount(string clientId, string accountType)
        {
            using var context = await _db.CreateDbContextAsync();
            return await CreateAccount(context, clientId, accountType);
        }

        public async Task<Account> CreateAccount(BankDataContext context, string clientId, string accountType)
        {
            var typeNormalized = ValidateInput(clientId, accountType); 

            await ValidateAccountCreationRules(context, clientId, typeNormalized); 

            var accountNumber = await GenerateAccountNumber(context);

            var account = new Account
            {
                AccountNumber = accountNumber,
                AccountType = typeNormalized,
                ClientId = clientId,
                CurrentBalance = 0.00m
            };

            context.AddOrUpdate(account); 
            await context.SaveChangesAsync();
            return account;
        }

       

        private string ValidateInput(string clientId, string accountType)
        {
           
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Client ID cannot be empty.", nameof(clientId));

            var typeNormalized = (accountType ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(typeNormalized))
                throw new ArgumentException("Account type cannot be empty.", nameof(accountType));

      
            var validTypes = _settings.ValidAccountTypes ?? Array.Empty<string>();
            if (validTypes.Length == 0)
                throw new InvalidOperationException("No valid account types configured.");

            if (!validTypes.Any(t => t.Equals(typeNormalized, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(
                    $"Invalid account type. Allowed types are: {string.Join(", ", validTypes)}",
                    nameof(accountType)
                );
            }
            return typeNormalized;
        }

        private async Task ValidateAccountCreationRules(BankDataContext context, string clientId, string typeNormalized)
        {
            
            var clientExists = await context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
                throw new InvalidOperationException($"No client found with the ID '{clientId}'.");

            var typeNormalizedLower = typeNormalized.ToLower();
            var alreadyExists = await context.Accounts
                .AnyAsync(a => a.ClientId == clientId && a.AccountType.ToLower() == typeNormalizedLower);
            if (alreadyExists)
                throw new InvalidOperationException($"Client already has an account of type '{typeNormalized}'.");
        }

       
        public async Task<List<AccountDetailsDto>> GetAllAccounts()
        {
            using var context = await _db.CreateDbContextAsync();

            
            var accounts = await context.Accounts
                .Include(a => a.Client)
                .ToListAsync();

            return _mapper.Map<List<AccountDetailsDto>>(accounts);
        }


        public async Task<AccountDetailsDto> GetAccountDetailsByAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be empty.", nameof(accountNumber));

            using var context = await _db.CreateDbContextAsync();

            var account = await context.Accounts
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new InvalidOperationException($"Account with number '{accountNumber}' not found.");

            return _mapper.Map<AccountDetailsDto>(account);
        }

      
       

        private async Task<string> GenerateAccountNumber(BankDataContext context)
        {
            
            var accountNumbers = await context.Accounts
                .Where(a => a.AccountNumber.Length == 7)
                .Select(a => a.AccountNumber)
                .ToListAsync();

            
            var lastNumber = accountNumbers
                .Where(n => n.Length == 7 && n.All(c => char.IsDigit(c)) && long.TryParse(n, out _))
                .Select(n => long.Parse(n))
                .DefaultIfEmpty(999999)
                .Max();

            var nextNumber = lastNumber >= 1000000 ? lastNumber + 1 : 1000000;
            return nextNumber.ToString("D7");
        }
    }
}