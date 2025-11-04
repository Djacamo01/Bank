using Lafise.API.data.model;
using Lafise.API.services.Clients.Dto;

namespace Lafise.API.services.Clients.Factories
{
    public interface IClientFactory
    {
        Client CreateClient(CreateClientDto clientDto, string normalizedTaxId);
    }
}

