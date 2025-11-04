using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Microsoft.EntityFrameworkCore;

namespace Lafise.API.services.Clients.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDbContextFactory<BankDataContext> _dbContextFactory;

        public ClientRepository(IDbContextFactory<BankDataContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory ?? throw new System.ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<bool> TaxIdExistsAsync(string taxId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await TaxIdExistsAsyncInternal(taxId, context);
        }

        public Task<bool> TaxIdExistsAsync(string taxId, BankDataContext context)
        {
            if (context == null)
                throw new System.ArgumentNullException(nameof(context));
            return TaxIdExistsAsyncInternal(taxId, context);
        }

        private async Task<bool> TaxIdExistsAsyncInternal(string taxId, BankDataContext context)
        {
            var normalizedTaxIdLower = taxId.ToLower();
            return await context.Clients
                .AnyAsync(c => c.TaxId.ToLower() == normalizedTaxIdLower);
        }

        public async Task<Client?> GetClientByIdAsync(string clientId)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Clients
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.Id == clientId);
        }

        public Task SaveClientAsync(Client client, BankDataContext context)
        {
            if (client == null)
                throw new System.ArgumentNullException(nameof(client));
            if (context == null)
                throw new System.ArgumentNullException(nameof(context));

            context.AddOrUpdate(client);
            return Task.CompletedTask;
        }
    }
}


