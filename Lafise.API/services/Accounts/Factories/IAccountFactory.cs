using Lafise.API.data.model;

namespace Lafise.API.services.Accounts.Factories
{

    public interface IAccountFactory
    {
        Account CreateAccount(string accountNumber, string accountType, string clientId);
    }
}

