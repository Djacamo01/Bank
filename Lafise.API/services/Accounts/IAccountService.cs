using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Transactions.Dto;

namespace Lafise.API.services.Accounts
{
    public interface  IAccountService
    {
        Task<AccountDto> CreateAccount(string clientId, string accountType);
        Task<AccountDto> CreateAccount(string accountType);
        Task<PagedDto<AccountDto>> GetAllAccounts(PaginationRequestDto pagination);
        Task<AccountDto> GetAccountDetailsByAccountNumber(string accountNumber);
        Task<AccountBalanceDto> GetAccountBalance();
        Task<AccountBalanceDto> GetAccountBalance(string accountNumber);
        Task<PagedDto<TransactionDto>> GetAccountMovements(string accountNumber, PaginationRequestDto pagination);
    }
}