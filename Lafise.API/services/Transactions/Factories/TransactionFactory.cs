using System;
using Lafise.API.data.model;

namespace Lafise.API.services.Transactions.Factories
{
    /// <summary>
    /// Factory for creating transaction entities following SOLID principles.
    /// </summary>
    public class TransactionFactory : ITransactionFactory
    {
        public Transaction CreateDepositTransaction(string accountId, decimal amount, decimal balanceAfter)
        {
            return CreateTransaction(accountId, "Deposit", amount, balanceAfter);
        }

        public Transaction CreateWithdrawalTransaction(string accountId, decimal amount, decimal balanceAfter)
        {
            return CreateTransaction(accountId, "Withdrawal", amount, balanceAfter);
        }

        public Transaction CreateTransferOutTransaction(string accountId, decimal amount, decimal balanceAfter)
        {
            return CreateTransaction(accountId, "Transfer Out", amount, balanceAfter);
        }

        public Transaction CreateTransferInTransaction(string accountId, decimal amount, decimal balanceAfter)
        {
            return CreateTransaction(accountId, "Transfer In", amount, balanceAfter);
        }

        private Transaction CreateTransaction(string accountId, string type, decimal amount, decimal balanceAfter)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                throw new ArgumentException("Account ID cannot be empty.", nameof(accountId));

            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Transaction type cannot be empty.", nameof(type));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            return new Transaction
            {
               
                AccountId = accountId,
                Type = type,
                Amount = amount,
                BalanceAfter = balanceAfter,
                Date = DateTime.UtcNow
            };
        }
    }
}

