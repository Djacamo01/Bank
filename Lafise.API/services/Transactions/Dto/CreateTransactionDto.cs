using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Transactions.Dto
{
    /// <summary>
    /// Data Transfer Object for creating a new transaction.
    /// </summary>
    public class CreateTransactionDto
    {
        /// <summary>
        /// Account number for the transaction.
        /// </summary>
        [Required]
        [Display(Description = "Account number")]
        [DefaultValue("1000000")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Amount for the transaction. Must be greater than zero.
        /// </summary>
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        [Display(Description = "Transaction amount")]
        [DefaultValue(200.00)]
        public decimal Amount { get; set; }
    }
}

