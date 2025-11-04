using System;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Lafise.API.utils;

namespace Lafise.API.services.Transactions.Validators
{
    /// <summary>
    /// Validates account-related operations following SOLID principles.
    /// </summary>
    public class AccountValidator : IAccountValidator
    {
        public Task ValidateAccountOwnership(Account account, string clientId, string operation)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Client ID cannot be empty.", nameof(clientId));

            if (account.ClientId != clientId)
            {
                var operationMessage = operation.ToLower() switch
                {
                    "deposit" => "deposit money to other accounts",
                    "withdraw" => "withdraw money from other accounts",
                    "transfer" => "transfer money from other accounts",
                    _ => "perform this operation on other accounts"
                };

                throw new LafiseException(403, 
                    $"You cannot {operationMessage}. Use the transfer functionality to send money to other accounts.");
            }

            return Task.CompletedTask;
        }

        public void ValidateSufficientBalance(Account account, decimal amount)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            if (account.CurrentBalance < amount)
            {
                throw new LafiseException(400,
                    $"Insufficient funds. Current balance: {account.CurrentBalance:C}, " +
                    $"Requested amount: {amount:C}");
            }
        }

        public void ValidateAccountExists(Account account, string accountNumber)
        {
            if (account == null)
            {
                throw new LafiseException(404, $"Account with number '{accountNumber}' not found.");
            }
        }
    }
}

