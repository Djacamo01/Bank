using System.Threading.Tasks;
using Lafise.API.data.model;

namespace Lafise.API.services.Transactions.Repositories
{
  
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByNumberAsync(string accountNumber);
    }
}

