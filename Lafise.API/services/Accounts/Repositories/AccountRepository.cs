using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Accounts.Repositories
{
    
    public class AccountRepository : IAccountRepository
    {
        private readonly IDbContextFactory<BankDataContext> _dbContextFactory;

        public AccountRepository(IDbContextFactory<BankDataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<Account?> GetAccountByNumberAsync(string accountNumber)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<Account?> GetAccountByClientIdAsync(string clientId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Accounts
                .FirstOrDefaultAsync(a => a.ClientId == clientId);
        }

        public async Task<List<Account>> GetAccountsByClientIdAsync(string clientId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Accounts
                .Where(a => a.ClientId == clientId)
                .ToListAsync();
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Accounts.ToListAsync();
        }

        public async Task<List<string>> GetAllAccountNumbersAsync()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Accounts
                .Where(a => a.AccountNumber.Length == 7)
                .Select(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task SaveAccountAsync(Account account)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            context.AddOrUpdate(account);
            await context.SaveChangesAsync();
        }
    }
}

