using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Transactions.Dto
{
    /// <summary>
    /// Data Transfer Object for transaction information.
    /// </summary>
    public class TransactionDto
    {
        /// <summary>
        /// Unique identifier of the transaction.
        /// </summary>
        [Display(Description = "Unique identifier of the transaction")]
        [DefaultValue("05f3daf7-234b-4e9c-839d-d7565e3a1871")]
        public required string Id { get; set; }

        /// <summary>
        /// Account number associated with the transaction.
        /// </summary>
        [Display(Description = "Account number")]
        [DefaultValue("1000000")]
        public required string AccountNumber { get; set; }

        /// <summary>
        /// Type of the transaction (e.g., Deposit, Withdrawal).
        /// </summary>
        [Display(Description = "Type of the transaction")]
        [DefaultValue("Deposit")]
        public required string Type { get; set; }

        /// <summary>
        /// Amount involved in the transaction.
        /// </summary>
        [Display(Description = "Transaction amount")]
        [DefaultValue(200.00)]
        public decimal Amount { get; set; }

        /// <summary>
        /// Account balance after the transaction.
        /// </summary>
        [Display(Description = "Balance after transaction")]
        [DefaultValue(1200.00)]
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// Date and time when the transaction occurred.
        /// </summary>
        [Display(Description = "Transaction date")]
        [DefaultValue("2023-01-01T12:34:56Z")]
        public DateTime Date { get; set; }
    }
}

