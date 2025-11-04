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
        Task<AccountDto> CreateAccount(string clientId, string accountType);
        Task<AccountDto> CreateAccount(string accountType);
        Task<List<AccountDto>> GetAllAccounts();
        Task<AccountDto> GetAccountDetailsByAccountNumber(string accountNumber);
    }
}