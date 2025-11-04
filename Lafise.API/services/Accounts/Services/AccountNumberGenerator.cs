using System;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.services.Accounts.Repositories;

namespace Lafise.API.services.Accounts.Services
{
    /// <summary>
    /// Generates unique account numbers following SOLID principles.
    /// </summary>
    public class AccountNumberGenerator : IAccountNumberGenerator
    {
        private readonly IAccountRepository _accountRepository;

        public AccountNumberGenerator(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task<string> GenerateAccountNumberAsync()
        {
            var accountNumbers = await _accountRepository.GetAllAccountNumbersAsync();

            var lastNumber = accountNumbers
                .Where(n => n.Length == 7 && n.All(c => char.IsDigit(c)) && long.TryParse(n, out _))
                .Select(n => long.Parse(n))
                .DefaultIfEmpty(999999)
                .Max();

            var nextNumber = lastNumber >= 1000000 ? lastNumber + 1 : 1000000;
            return nextNumber.ToString("D7");
        }
    }
}

