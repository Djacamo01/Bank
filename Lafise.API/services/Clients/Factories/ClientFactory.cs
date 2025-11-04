using System;
using Lafise.API.data.model;
using Lafise.API.services.Clients.Dto;

namespace Lafise.API.services.Clients.Factories
{
    public class ClientFactory : IClientFactory
    {
        public Client CreateClient(CreateClientDto clientDto, string normalizedTaxId)
        {
            if (clientDto == null)
                throw new ArgumentNullException(nameof(clientDto));
            if (string.IsNullOrWhiteSpace(normalizedTaxId))
                throw new ArgumentException("Normalized Tax ID cannot be empty.", nameof(normalizedTaxId));

            return new Client
            {
                Name = clientDto.Name,
                LastName = clientDto.LastName,
                TaxId = normalizedTaxId,
                Email = clientDto.Email,
                DateOfBirth = clientDto.DateOfBirth,
                Gender = clientDto.Gender,
                Income = clientDto.Income
            };
        }
    }
}

