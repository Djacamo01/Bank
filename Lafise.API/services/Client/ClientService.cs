using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.client;
using Microsoft.EntityFrameworkCore;
namespace Lafise.API.services.client
{
    public class ClientService:IClientService
    {
        private readonly IDbContextFactory<BankDataContext> _db;

        public ClientService(IDbContextFactory<BankDataContext> db)
        {
            _db = db;
        }

        public async Task<List<Client>> GetAllClients()
        {
            using var context = _db.CreateDbContext();
            return await context.Clients.ToListAsync();
        }
    }

    public interface IClientService
    {
         Task<List<Client>> GetAllClients();
    }
}