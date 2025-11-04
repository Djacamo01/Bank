using System;
using Lafise.API.services.Transactions.Dto;
using Lafise.API.utils;

namespace Lafise.API.services.Transactions.Validators
{
    /// <summary>
    /// Validates transaction requests following SOLID principles.
    /// </summary>
    public class TransactionValidator : ITransactionValidator
    {
        public void ValidateTransactionRequest(CreateTransactionDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.AccountNumber))
                throw new LafiseException(400, "Account number cannot be empty.");

            if (request.Amount <= 0)
                throw new LafiseException(400, "Transaction amount must be greater than zero.");
        }

        public void ValidateTransferRequest(TransferDto request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.FromAccountNumber))
                throw new LafiseException(400, "Source account number cannot be empty.");

            if (string.IsNullOrWhiteSpace(request.ToAccountNumber))
                throw new LafiseException(400, "Destination account number cannot be empty.");

            if (request.FromAccountNumber == request.ToAccountNumber)
                throw new LafiseException(400, "Source and destination accounts cannot be the same.");

            if (request.Amount <= 0)
                throw new LafiseException(400, "Transfer amount must be greater than zero.");
        }

        public void ValidateAuthentication(string clientId)
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(401, "User not authenticated.");
        }
    }
}

