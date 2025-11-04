using System.Threading.Tasks;

namespace Lafise.API.services.Clients.Validators
{
    public interface IClientCreationValidator
    {
        string ValidateAndNormalizeTaxId(string taxId);
        Task ValidateTaxIdNotDuplicateAsync(string normalizedTaxId);
        Task ValidateTaxIdNotDuplicateAsync(string normalizedTaxId, Lafise.API.data.BankDataContext context);
        void ValidateAccountTypeRequired(string accountType);
    }
}

