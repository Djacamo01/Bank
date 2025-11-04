using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Transactions.Dto
{
    /// <summary>
    /// Data Transfer Object for transferring funds between accounts.
    /// </summary>
    public class TransferDto
    {
        /// <summary>
        /// Source account number (account to transfer from).
        /// </summary>
        [Required]
        [Display(Description = "Source account number")]
        [DefaultValue("1000000")]
        public string FromAccountNumber { get; set; }

        /// <summary>
        /// Destination account number (account to transfer to).
        /// </summary>
        [Required]
        [Display(Description = "Destination account number")]
        [DefaultValue("1000001")]
        public string ToAccountNumber { get; set; }

        /// <summary>
        /// Amount to transfer. Must be greater than zero.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        [Display(Description = "Transfer amount")]
        [DefaultValue(200.00)]
        public decimal Amount { get; set; }
    }
}

