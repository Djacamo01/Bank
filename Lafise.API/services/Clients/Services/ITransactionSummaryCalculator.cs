using System.Collections.Generic;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Clients.Dto;
using Lafise.API.services.Transactions.Dto;

namespace Lafise.API.services.Clients.Services
{
    public interface ITransactionSummaryCalculator
    {
        Task<TransactionSummaryDto> CalculateAccountSummaryAsync(Account account, BankDataContext context);
        Task<List<AccountSummaryDto>> CalculateAccountsSummariesAsync(List<Account> accounts, BankDataContext context);
    }
}


