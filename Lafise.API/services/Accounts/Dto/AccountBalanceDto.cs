using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Accounts.Dto
{
    /// <summary>
    /// Data Transfer Object for account balance information.
    /// </summary>
    public class AccountBalanceDto
    {
        /// <summary>
        /// Account number.
        /// </summary>
        [Display(Description = "Account number")]
        [DefaultValue("1000000")]
        public string AccountNumber { get; set; } = string.Empty;

        /// <summary>
        /// Account type (e.g., Savings, Checking).
        /// </summary>
        [Display(Description = "Account type")]
        [DefaultValue("Savings")]
        public string AccountType { get; set; } = string.Empty;

        /// <summary>
        /// Current balance of the account.
        /// </summary>
        [Display(Description = "Current balance")]
        [DefaultValue(12500.50)]
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Currency code (e.g., USD, EUR).
        /// </summary>
        [Display(Description = "Currency code")]
        [DefaultValue("USD")]
        public string Currency { get; set; } = "USD";

        /// <summary>
        /// Date and time when the balance was last updated.
        /// </summary>
        [Display(Description = "Last updated date")]
        [DefaultValue("2023-01-01T12:34:56Z")]
        public DateTime LastUpdated { get; set; }
    }
}

