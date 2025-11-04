using Lafise.API.data.model;

namespace Lafise.API.services.Transactions.Factories
{
    
    public interface ITransactionFactory
    {
       
        Transaction CreateDepositTransaction(string accountId, decimal amount, decimal balanceAfter);

        Transaction CreateWithdrawalTransaction(string accountId, decimal amount, decimal balanceAfter);

       
        Transaction CreateTransferOutTransaction(string accountId, decimal amount, decimal balanceAfter);

        Transaction CreateTransferInTransaction(string accountId, decimal amount, decimal balanceAfter);
    }
}

