using System.Collections.Generic;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.services.Transactions.Dto;

namespace Lafise.API.services.Transactions
{
    public interface ITransactionService
    {
        Task<TransactionDto> Deposit(CreateTransactionDto request);

        Task<TransactionDto> Withdraw(CreateTransactionDto request);

        Task<PagedDtoSummary<TransactionDto, TransactionSummaryDto>> GetAccountMovements(string accountNumber, PaginationRequestDto pagination);


        Task<TransactionDto> Transfer(TransferDto request);
    }
}

