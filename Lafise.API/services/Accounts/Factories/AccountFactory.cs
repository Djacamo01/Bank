using System;
using Lafise.API.data.model;

namespace Lafise.API.services.Accounts.Factories
{
    /// <summary>
    /// Factory for creating account entities following SOLID principles.
    /// </summary>
    public class AccountFactory : IAccountFactory
    {
        public Account CreateAccount(string accountNumber, string accountType, string clientId)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number cannot be empty.", nameof(accountNumber));

            if (string.IsNullOrWhiteSpace(accountType))
                throw new ArgumentException("Account type cannot be empty.", nameof(accountType));

            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("Client ID cannot be empty.", nameof(clientId));

            return new Account
            {
                AccountNumber = accountNumber,
                AccountType = accountType,
                ClientId = clientId,
                CurrentBalance = 0.00m
            };
        }
    }
}

