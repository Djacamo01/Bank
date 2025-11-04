using System.Threading.Tasks;
using Lafise.API.data.model;

namespace Lafise.API.services.Transactions.Validators
{
    /// <summary>
    /// Interface for validating account-related operations.
    /// </summary>
    public interface IAccountValidator
    {
    
        Task ValidateAccountOwnership(Account account, string clientId, string operation);

        void ValidateSufficientBalance(Account account, decimal amount);

        void ValidateAccountExists(Account account, string accountNumber);
    }
}

