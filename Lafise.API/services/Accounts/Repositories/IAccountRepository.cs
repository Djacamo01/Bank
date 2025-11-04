using System.Collections.Generic;
using System.Threading.Tasks;
using Lafise.API.data.model;

namespace Lafise.API.services.Accounts.Repositories
{
    
    public interface IAccountRepository
    {
        
        Task<Account?> GetAccountByNumberAsync(string accountNumber);

        Task<Account?> GetAccountByClientIdAsync(string clientId);

        Task<List<Account>> GetAllAccountsAsync();

        Task<List<string>> GetAllAccountNumbersAsync();

        Task SaveAccountAsync(Account account);
    }
}

