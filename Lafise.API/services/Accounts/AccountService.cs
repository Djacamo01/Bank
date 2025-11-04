using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Auth;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly IDbContextFactory<BankDataContext> _db;
        private readonly AccountSettings _settings;
        private readonly IMapper _mapper;
        private readonly IAuthInfo _authInfo;

        public AccountService(IDbContextFactory<BankDataContext> db, AccountSettings settings, IMapper mapper, IAuthInfo authInfo)
        {
            _db = db;
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authInfo = authInfo ?? throw new ArgumentNullException(nameof(authInfo));
        }


        public async Task<AccountDto> CreateAccount(string accountType)
        {
            var clientId = _authInfo.UserId();
            
            return await CreateAccount(clientId, accountType);
        }



        public async Task<AccountDto> CreateAccount(string clientId, string accountType)
        {
            using var context = await _db.CreateDbContextAsync();
            return await CreateAccount(context, clientId, accountType);
        }
        

        public async Task<AccountDto> CreateAccount(BankDataContext context, string clientId, string accountType)
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
            return _mapper.Map<AccountDto>(account);
        }



        private string ValidateInput(string clientId, string accountType)
        {

            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(400, "Client ID cannot be empty.");

            var typeNormalized = (accountType ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(typeNormalized))
                throw new LafiseException(400, "Account type cannot be empty.");


            var validTypes = _settings.ValidAccountTypes ?? Array.Empty<string>();
            if (validTypes.Length == 0)
                throw new LafiseException(500, "No valid account types configured.");

            if (!validTypes.Any(t => t.Equals(typeNormalized, StringComparison.OrdinalIgnoreCase)))
            {
                throw new LafiseException(400, 
                    $"Invalid account type '{accountType}'. Allowed types are: {string.Join(", ", validTypes)}");
            }
            return typeNormalized;
        }

        private async Task ValidateAccountCreationRules(BankDataContext context, string clientId, string typeNormalized)
        {
            // Validar que el cliente existe
            var clientExists = await context.Clients.AnyAsync(c => c.Id == clientId);
            if (!clientExists)
                throw new LafiseException(404, $"No client found with the ID '{clientId}'.");

            // Validar que el usuario no tenga ya una cuenta de este tipo
            // Un usuario solo puede tener 1 cuenta de cada tipo disponible
            var typeNormalizedLower = typeNormalized.ToLower();
            var existingAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.ClientId == clientId && a.AccountType.ToLower() == typeNormalizedLower);
            
            if (existingAccount != null)
            {
               
                var existingAccountTypes = await context.Accounts
                    .Where(a => a.ClientId == clientId)
                    .Select(a => a.AccountType)
                    .ToListAsync();

                var availableTypes = _settings.ValidAccountTypes ?? Array.Empty<string>();
                var availableTypesList = string.Join(", ", availableTypes);
                var existingTypesList = string.Join(", ", existingAccountTypes);

                throw new LafiseException(400, 
                    $"You already have a '{typeNormalized}' account. " +
                    $"Each user can only have one account of each type. " +
                    $"Available account types: {availableTypesList}. " +
                    $"Your existing accounts: {existingTypesList}.");
            }
        }


        public async Task<List<AccountDto>> GetAllAccounts()
        {
            using var context = await _db.CreateDbContextAsync();

            var accounts = await context.Accounts
                .ToListAsync();

            return _mapper.Map<List<AccountDto>>(accounts);
        }


        public async Task<AccountDto> GetAccountDetailsByAccountNumber(string accountNumber)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new LafiseException(400, "Account number cannot be empty.");

            using var context = await _db.CreateDbContextAsync();

            var account = await context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

            if (account == null)
                throw new LafiseException(404, $"Account with number '{accountNumber}' not found.");

            return _mapper.Map<AccountDto>(account);
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