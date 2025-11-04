using System.Threading.Tasks;

namespace Lafise.API.services.Accounts.Services
{
    
    public interface IAccountNumberGenerator
    {
       
        Task<string> GenerateAccountNumberAsync();
    }
}

