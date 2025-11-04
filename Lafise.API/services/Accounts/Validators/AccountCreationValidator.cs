using System;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Accounts.Validators
{

    public class AccountCreationValidator : IAccountCreationValidator
    {
        private readonly IDbContextFactory<BankDataContext> _dbContextFactory;

        public AccountCreationValidator(IDbContextFactory<BankDataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public string ValidateAndNormalizeAccountType(string accountType, string[] validAccountTypes)
        {
            if (string.IsNullOrWhiteSpace(accountType))
                throw new LafiseException(400, "Account type cannot be empty.");

            if (validAccountTypes == null || validAccountTypes.Length == 0)
                throw new LafiseException(500, "No valid account types configured.");

            var typeNormalized = accountType.Trim();
            if (!validAccountTypes.Any(t => t.Equals(typeNormalized, StringComparison.OrdinalIgnoreCase)))
            {
                throw new LafiseException(400,
                    $"Invalid account type '{accountType}'. Allowed types are: {string.Join(", ", validAccountTypes)}");
            }

            return typeNormalized;
        }

        public async Task ValidateClientExistsAsync(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(400, "Client ID cannot be empty.");

            using var context = await _dbContextFactory.CreateDbContextAsync();
            var clientExists = await context.Clients.AnyAsync(c => c.Id == clientId);
            
            if (!clientExists)
                throw new LafiseException(404, $"No client found with the ID '{clientId}'.");
        }

        public Task ValidateClientExistsAsync(string clientId, BankDataContext context)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(400, "Client ID cannot be empty.");
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return ValidateClientExistsAsyncInternal(clientId, context);
        }

        private async Task ValidateClientExistsAsyncInternal(string clientId, BankDataContext context)
        {
            var clientExists = await context.Clients.AnyAsync(c => c.Id == clientId);
            
            if (!clientExists)
                throw new LafiseException(404, $"No client found with the ID '{clientId}'.");
        }

        public async Task ValidateNoDuplicateAccountTypeAsync(string clientId, string accountType, string[] validAccountTypes)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            await ValidateNoDuplicateAccountTypeAsyncInternal(clientId, accountType, validAccountTypes, context);
        }

        public Task ValidateNoDuplicateAccountTypeAsync(string clientId, string accountType, string[] validAccountTypes, BankDataContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return ValidateNoDuplicateAccountTypeAsyncInternal(clientId, accountType, validAccountTypes, context);
        }

        private async Task ValidateNoDuplicateAccountTypeAsyncInternal(string clientId, string accountType, string[] validAccountTypes, BankDataContext context)
        {
            var typeNormalizedLower = accountType.ToLower();
            var existingAccount = await context.Accounts
                .FirstOrDefaultAsync(a => a.ClientId == clientId && a.AccountType.ToLower() == typeNormalizedLower);
            
            if (existingAccount != null)
            {
                var existingAccountTypes = await context.Accounts
                    .Where(a => a.ClientId == clientId)
                    .Select(a => a.AccountType)
                    .ToListAsync();

                var availableTypesList = string.Join(", ", existingAccountTypes);
                var existingTypesList = string.Join(", ", existingAccountTypes);

                throw new LafiseException(400,
                    $"You already have a '{accountType}' account. " +
                    $"Each user can only have one account of each type. " +
                    $"Available account types: {availableTypesList}. " +
                    $"Your existing accounts: {existingTypesList}.");
            }
        }
    }
}

