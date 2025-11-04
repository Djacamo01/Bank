using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lafise.API.controllers.Dto;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Repositories;
using Lafise.API.services.Auth;
using Lafise.API.services.Clients;
using Lafise.API.services.Clients.Dto;
using Lafise.API.services.Clients.Factories;
using Lafise.API.services.Clients.Repositories;
using Lafise.API.services.Clients.Services;
using Lafise.API.services.Clients.Validators;
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
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthInfo _authInfo;
        private readonly IClientRepository _clientRepository;
        private readonly IClientCreationValidator _clientCreationValidator;
        private readonly IClientFactory _clientFactory;
        private readonly ITransactionSummaryCalculator _transactionSummaryCalculator;

        public ClientService(
            IDbContextFactory<BankDataContext> db, 
            IAccountService accountService, 
            ICryptor cryptor, 
            IMapper mapper,
            IAccountRepository accountRepository,
            IAuthInfo authInfo,
            IClientRepository clientRepository,
            IClientCreationValidator clientCreationValidator,
            IClientFactory clientFactory,
            ITransactionSummaryCalculator transactionSummaryCalculator)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
            _cryptor = cryptor ?? throw new ArgumentNullException(nameof(cryptor));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            _authInfo = authInfo ?? throw new ArgumentNullException(nameof(authInfo));
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _clientCreationValidator = clientCreationValidator ?? throw new ArgumentNullException(nameof(clientCreationValidator));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _transactionSummaryCalculator = transactionSummaryCalculator ?? throw new ArgumentNullException(nameof(transactionSummaryCalculator));
        }

       

        public async Task<ClientResponseDto> CreateClient(CreateClientDto client)
        {
            using var context = await _db.CreateDbContextAsync();

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                
                var normalizedTaxId = _clientCreationValidator.ValidateAndNormalizeTaxId(client.TaxId);

               
                await _clientCreationValidator.ValidateTaxIdNotDuplicateAsync(normalizedTaxId, context);

                
                _clientCreationValidator.ValidateAccountTypeRequired(client.AccountType);

                
                var newClient = _clientFactory.CreateClient(client, normalizedTaxId);

               
                _cryptor.ValidatePassword(client.Password);
                _cryptor.CreatePasswordHash(client.Password, newClient);

             
                await _clientRepository.SaveClientAsync(newClient, context);
                await context.SaveChangesAsync();

               
                var createdAccount = await _accountService.CreateAccount(newClient.Id, client.AccountType, context);

                await transaction.CommitAsync();
                
                var response = _mapper.Map<ClientResponseDto>(newClient);
                response.AccountNumber = createdAccount.AccountNumber;
                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ClientAccountsSummaryDto> GetClientAccountsSummary()
        {
            
            var clientId = _authInfo.UserId();
            if (string.IsNullOrWhiteSpace(clientId))
                throw new LafiseException(401, "User not authenticated.");

            using var context = await _db.CreateDbContextAsync();

           
            var client = await _clientRepository.GetClientByIdAsync(clientId);
            if (client == null)
                throw new LafiseException(404, $"Client with ID '{clientId}' not found.");

          
            var accounts = await _accountRepository.GetAccountsByClientIdAsync(clientId);

           
            var accountSummaries = await _transactionSummaryCalculator.CalculateAccountsSummariesAsync(accounts, context);

           
            var summary = new ClientAccountsSummaryDto
            {
                ClientId = client.Id,
                FullName = $"{client.Name} {client.LastName}",
                Email = client.Email,
                Accounts = accountSummaries,
                TotalAccounts = accounts.Count,
                TotalBalance = accounts.Sum(a => a.CurrentBalance)
            };

            return summary;
        }

    }

   
}