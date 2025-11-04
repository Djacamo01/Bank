using System;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Transactions.Repositories
{
    /// <summary>
    /// Repository for account data access following SOLID principles.
    /// </summary>
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
    }
}

