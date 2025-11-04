using System;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.services.Clients.Repositories;
using Lafise.API.utils;

namespace Lafise.API.services.Clients.Validators
{
    public class ClientCreationValidator : IClientCreationValidator
    {
        private readonly IClientRepository _clientRepository;

        public ClientCreationValidator(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
        }

        public string ValidateAndNormalizeTaxId(string taxId)
        {
            if (string.IsNullOrWhiteSpace(taxId))
            {
                throw new ArgumentException("Tax ID cannot be empty.", nameof(taxId));
            }

            // Normalizar TaxId (eliminar espacios y guiones comunes)
            var normalizedTaxId = taxId.Trim().Replace("-", "").Replace(" ", "");

            // Validar formato básico (solo números y letras, longitud mínima)
            if (normalizedTaxId.Length < 5)
            {
                throw new ArgumentException("Tax ID must be at least 5 characters long.", nameof(taxId));
            }

            // Validar que contenga solo caracteres válidos
            if (!normalizedTaxId.All(c => char.IsLetterOrDigit(c)))
            {
                throw new ArgumentException("Tax ID can only contain letters and numbers.", nameof(taxId));
            }

            return normalizedTaxId;
        }

        public async Task ValidateTaxIdNotDuplicateAsync(string normalizedTaxId)
        {
            if (string.IsNullOrWhiteSpace(normalizedTaxId))
                throw new ArgumentException("Tax ID cannot be empty.", nameof(normalizedTaxId));

            var taxIdExists = await _clientRepository.TaxIdExistsAsync(normalizedTaxId);
            if (taxIdExists)
            {
                throw new LafiseException(400, $"A client with Tax ID '{normalizedTaxId}' already exists.");
            }
        }

        public async Task ValidateTaxIdNotDuplicateAsync(string normalizedTaxId, BankDataContext context)
        {
            if (string.IsNullOrWhiteSpace(normalizedTaxId))
                throw new ArgumentException("Tax ID cannot be empty.", nameof(normalizedTaxId));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var taxIdExists = await _clientRepository.TaxIdExistsAsync(normalizedTaxId, context);
            if (taxIdExists)
            {
                throw new LafiseException(400, $"A client with Tax ID '{normalizedTaxId}' already exists.");
            }
        }

        public void ValidateAccountTypeRequired(string accountType)
        {
            if (string.IsNullOrWhiteSpace(accountType))
            {
                throw new ArgumentException("Account type is required when creating a client.", nameof(accountType));
            }
        }
    }
}


