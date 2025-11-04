using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Accounts.Dto
{
    /// <summary>
    /// Data Transfer Object containing detailed information of an account.
    /// </summary>
    /// <remarks>
    /// This class is used to provide all necessary account details, including:
    /// - Account identifier and metadata
    /// - Account type and balance
    /// - Owner information
    /// </remarks>
    public class AccountDto
    {
        /// <summary>
        /// Unique identifier for the account.
        /// </summary>
        [Display(Description = "Unique identifier of the account")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public string Id { get; set; }

        /// <summary>
        /// Unique account number.
        /// </summary>
        [Display(Description = "Unique account number")]
        [DefaultValue("10001111222")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Type of the account (e.g., Savings, Checking).
        /// </summary>
        [Display(Description = "Type of the account, such as Savings or Checking")]
        [DefaultValue("Checking")]
        public string AccountType { get; set; }

        /// <summary>
        /// Current balance of the account.
        /// </summary>
        [Display(Description = "Current balance of the account")]
        [DefaultValue(12500.50)]
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// Date and time when the account was created.
        /// </summary>
        [Display(Description = "Date and time when the account was created")]
        [DefaultValue("2023-01-01T12:34:56Z")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time when the account was last modified, if applicable.
        /// </summary>
        [Display(Description = "Date and time when the account was last modified")]
        [DefaultValue("2023-02-01T09:30:00Z")]
        public DateTime? DateModified { get; set; }


    }



}
