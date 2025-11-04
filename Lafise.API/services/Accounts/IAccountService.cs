using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;

namespace Lafise.API.services.Accounts
{
    public interface   IAccountService
    {
        Task<Account> CreateAccount(string clientId, string accountType);
        Task<Account> CreateAccount(BankDataContext context, string clientId, string accountType);
        Task<List<AccountDetailsDto>> GetAllAccounts();
        Task<AccountDetailsDto> GetAccountDetailsByAccountNumber(string accountNumber);
    }
}