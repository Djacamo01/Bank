using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts;
using Lafise.API.services.Clients.Dto;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;
namespace Lafise.API.services.clients
{
    public class ClientService : IClientService
    {
        private readonly IDbContextFactory<BankDataContext> _db;
        private readonly IAccountService _accountService;
        private readonly ICryptor _cryptor;
        private readonly IMapper _mapper;

        public ClientService(IDbContextFactory<BankDataContext> db, IAccountService accountService, ICryptor cryptor, IMapper mapper)
        {
            _db = db;
            _accountService = accountService;
            _cryptor = cryptor;
            _mapper = mapper;
        }

        public async Task<List<Client>> GetAllClients()
        {
            using var context = await _db.CreateDbContextAsync();
            return await context.Clients.ToListAsync();
        }

        public async Task<ClientResponseDto> CreateClient(CreateClientDto client)
        {
            using (var context = await _db.CreateDbContextAsync())
            {

                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    try
                    {

                        var normalizedTaxId = ValidateAndNormalizeTaxId(client.TaxId);


                        var normalizedTaxIdLower = normalizedTaxId.ToLower();
                        var taxIdExists = await context.Clients
                            .AnyAsync(c => c.TaxId.ToLower() == normalizedTaxIdLower);
                        if (taxIdExists)
                        {
                            throw new InvalidOperationException($"A client with Tax ID '{normalizedTaxId}' already exists.");
                        }

                        var newClient = new Client
                        {
                            Name = client.Name,
                            LastName = client.LastName,
                            TaxId = normalizedTaxId,
                            Email = client.Email,
                            DateOfBirth = client.DateOfBirth,
                            Gender = client.Gender,
                            Income = client.Income,

                        };
                        _cryptor.ValidatePassword(client.Password);
                        _cryptor.CreatePasswordHash(client.Password, newClient);
                        context.AddOrUpdate(newClient);
                        await context.SaveChangesAsync();


                        if (!string.IsNullOrWhiteSpace(client.AccountType))
                        {
                            await _accountService.CreateAccount(context, newClient.Id, client.AccountType);
                        }


                        await transaction.CommitAsync();
                        return _mapper.Map<ClientResponseDto>(newClient);
                    }
                    catch
                    {
                        // Si algo falla, hacer rollback de toda la transacción
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
        }

        private string ValidateAndNormalizeTaxId(string taxId)
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






    }

    public interface IClientService
    {
        Task<List<Client>> GetAllClients();
        Task<ClientResponseDto> CreateClient(CreateClientDto client);
    }
}