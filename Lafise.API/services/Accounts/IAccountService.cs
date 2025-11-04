using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Transactions.Dto;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Accounts
{
    public interface  IAccountService
    {
        Task<AccountDto> CreateAccount(string clientId, string accountType);
        Task<AccountDto> CreateAccount(string clientId, string accountType, BankDataContext context);
        Task<AccountDto> CreateAccount(string accountType);
        Task<AccountDto> GetAccountDetailsByAccountNumber(string accountNumber);
        Task<AccountBalanceDto> GetAccountBalance();
        Task<AccountBalanceDto> GetAccountBalance(string accountNumber);
        Task<PagedDtoSummary<TransactionDto, TransactionSummaryDto>> GetAccountMovements(string accountNumber, PaginationRequestDto pagination);
    }
}