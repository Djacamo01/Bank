using System.Threading.Tasks;
using Lafise.API.data;

namespace Lafise.API.services.Accounts.Validators
{
   
    public interface IAccountCreationValidator
    {
       
        string ValidateAndNormalizeAccountType(string accountType, string[] validAccountTypes);

        Task ValidateClientExistsAsync(string clientId);
        Task ValidateClientExistsAsync(string clientId, BankDataContext context);

        Task ValidateNoDuplicateAccountTypeAsync(string clientId, string accountType, string[] validAccountTypes);
        Task ValidateNoDuplicateAccountTypeAsync(string clientId, string accountType, string[] validAccountTypes, BankDataContext context);
    }
}

