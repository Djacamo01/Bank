using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.services.Clients.Dto;

namespace Lafise.API.services.Clients
{
     public interface IClientService
    {
        
        Task<ClientResponseDto> CreateClient(CreateClientDto client);
        
        Task<ClientAccountsSummaryDto> GetClientAccountsSummary();
    }
}