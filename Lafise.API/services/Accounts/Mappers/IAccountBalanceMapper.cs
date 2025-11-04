using Lafise.API.data.model;
using Lafise.API.services.Accounts.Dto;

namespace Lafise.API.services.Accounts.Mappers
{
 
    public interface IAccountBalanceMapper
    {
     
        AccountBalanceDto MapToBalanceDto(Account account);
    }
}

