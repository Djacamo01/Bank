using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;

namespace Lafise.API.services.Clients.Repositories
{
    public interface IClientRepository
    {
        Task<bool> TaxIdExistsAsync(string taxId);
        Task<bool> TaxIdExistsAsync(string taxId, BankDataContext context);
        Task<Client?> GetClientByIdAsync(string clientId);
        Task SaveClientAsync(Client client, BankDataContext context);
    }
}


