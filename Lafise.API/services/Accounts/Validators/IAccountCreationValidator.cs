using System.Threading.Tasks;

namespace Lafise.API.services.Accounts.Validators
{
   
    public interface IAccountCreationValidator
    {
       
        string ValidateAndNormalizeAccountType(string accountType, string[] validAccountTypes);

        Task ValidateClientExistsAsync(string clientId);

        Task ValidateNoDuplicateAccountTypeAsync(string clientId, string accountType, string[] validAccountTypes);
    }
}

