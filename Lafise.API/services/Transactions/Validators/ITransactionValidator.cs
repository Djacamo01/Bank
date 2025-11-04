using Lafise.API.services.Transactions.Dto;

namespace Lafise.API.services.Transactions.Validators
{
    /// <summary>
    /// Interface for validating transaction requests.
    /// </summary>
    public interface ITransactionValidator
    {
           void ValidateTransactionRequest(CreateTransactionDto request);

  
        void ValidateTransferRequest(TransferDto request);

        void ValidateAuthentication(string clientId);
    }
}

