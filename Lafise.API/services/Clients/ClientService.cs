using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Clients.Dto;
using Microsoft.EntityFrameworkCore;
namespace Lafise.API.services.clients
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
            using var context = await _db.CreateDbContextAsync();
            return await context.Clients.ToListAsync();
        }

        public async Task<Client> CreateClient(CreateClientDto client)
        {



        }
    



    }

    public interface IClientService
    {
         Task<List<Client>> GetAllClients();
    }
}