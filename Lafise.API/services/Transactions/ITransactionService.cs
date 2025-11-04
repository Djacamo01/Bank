using System.Collections.Generic;
using System.Threading.Tasks;
using Lafise.API.services.Transactions.Dto;

namespace Lafise.API.services.Transactions
{
    public interface ITransactionService
    {
        Task<TransactionDto> Deposit(CreateTransactionDto request);

        Task<TransactionDto> Withdraw(CreateTransactionDto request);

        Task<List<TransactionDto>> GetAccountMovements(string accountNumber);


        Task<TransactionDto> Transfer(TransferDto request);
    }
}

