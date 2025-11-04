using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Clients.Dto;
using Lafise.API.services.Transactions.Dto;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Clients.Services
{
    public class TransactionSummaryCalculator : ITransactionSummaryCalculator
    {
        public async Task<TransactionSummaryDto> CalculateAccountSummaryAsync(Account account, BankDataContext context)
        {
            if (account == null)
                throw new ArgumentNullException(nameof(account));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var transactions = await context.Transactions
                .Where(t => t.AccountId == account.Id)
                .ToListAsync();

            var transactionSummary = new TransactionSummaryDto
            {
                TotalDeposits = transactions.Count(t => t.Type.Equals("Deposit", StringComparison.OrdinalIgnoreCase)),
                TotalDepositsAmount = transactions
                    .Where(t => t.Type.Equals("Deposit", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                TotalWithdrawals = transactions.Count(t => t.Type.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase)),
                TotalWithdrawalsAmount = transactions
                    .Where(t => t.Type.Equals("Withdrawal", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                TotalTransfersOut = transactions.Count(t => t.Type.Equals("Transfer Out", StringComparison.OrdinalIgnoreCase)),
                TotalTransfersOutAmount = transactions
                    .Where(t => t.Type.Equals("Transfer Out", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                TotalTransfersIn = transactions.Count(t => t.Type.Equals("Transfer In", StringComparison.OrdinalIgnoreCase)),
                TotalTransfersInAmount = transactions
                    .Where(t => t.Type.Equals("Transfer In", StringComparison.OrdinalIgnoreCase))
                    .Sum(t => t.Amount),
                
                CurrentBalance = account.CurrentBalance
            };

            transactionSummary.NetAmount = (transactionSummary.TotalDepositsAmount + transactionSummary.TotalTransfersInAmount) 
                - (transactionSummary.TotalWithdrawalsAmount + transactionSummary.TotalTransfersOutAmount);

            return transactionSummary;
        }

        public async Task<List<AccountSummaryDto>> CalculateAccountsSummariesAsync(List<Account> accounts, BankDataContext context)
        {
            if (accounts == null)
                throw new ArgumentNullException(nameof(accounts));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var accountSummaries = new List<AccountSummaryDto>();

            foreach (var account in accounts)
            {
                var transactionSummary = await CalculateAccountSummaryAsync(account, context);
                
                var transactions = await context.Transactions
                    .Where(t => t.AccountId == account.Id)
                    .ToListAsync();

                var lastTransaction = transactions
                    .OrderByDescending(t => t.Date)
                    .FirstOrDefault();

                var accountSummary = new AccountSummaryDto
                {
                    AccountNumber = account.AccountNumber,
                    AccountType = account.AccountType,
                    CurrentBalance = account.CurrentBalance,
                    DateCreated = account.DateCreated,
                    TransactionSummary = transactionSummary,
                    TotalTransactions = transactions.Count,
                    LastTransactionDate = lastTransaction?.Date
                };

                accountSummaries.Add(accountSummary);
            }

            return accountSummaries;
        }
    }
}


