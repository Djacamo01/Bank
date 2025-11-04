using System;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;

namespace Lafise.API.services.Accounts.Mappers
{
    
    public class AccountBalanceMapper : IAccountBalanceMapper
    {
        public AccountBalanceDto MapToBalanceDto(Account account)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));

            return new AccountBalanceDto
            {
                AccountNumber = account.AccountNumber,
                AccountType = account.AccountType,
                CurrentBalance = account.CurrentBalance,
                Currency = "USD",
                LastUpdated = account.DateModified ?? account.DateCreated
            };
        }
    }
}

