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
    public class AccountDetailsDto
    {
        /// <summary>
        /// Unique identifier for the account.
        /// </summary>
        [Display(Description = "Unique identifier of the account")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public string  Id { get; set; }

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

        /// <summary>
        /// Information about the client who owns the account.
        /// </summary>
        [Display(Description = "Information about the client who owns the account")]
        public ClientInfoDto Owner { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for holding basic client information.
    /// </summary>
    /// <remarks>
    /// Encapsulates identifying and personal information about a client:
    /// - Basic identity fields
    /// - Contact and demographic details
    /// </remarks>
    public class ClientInfoDto
    {
        /// <summary>
        /// Unique identifier for the client.
        /// </summary>
        [Display(Description = "Unique identifier of the authenticated user")]
        [DefaultValue("51b9ecc1-f701-4f67-8554-73ff0db565fc")]
        public string Id { get; set; }

        /// <summary>
        /// Client's first name.
        /// </summary>
        [Display(Description = "Client's first name")]
        [DefaultValue("Juan")]
        public string Name { get; set; }

        /// <summary>
        /// Client's last name.
        /// </summary>
        [Display(Description = "Client's last name")]
        [DefaultValue("Perez")]
        public string LastName { get; set; }

        /// <summary>
        /// Tax identification number of the client.
        /// </summary>
        [Display(Description = "Tax identification number of the client")]
        [DefaultValue("001-231019-0007E")]
        public string TaxId { get; set; }

        /// <summary>
        /// Email address of the client.
        /// </summary>
        [Display(Description = "Email address of the client")]
        [DefaultValue("juan.perez@email.com")]
        public string? Email { get; set; }

        /// <summary>
        /// Client's date of birth.
        /// </summary>
        [Display(Description = "Client's date of birth")]
        [DefaultValue("1991-10-23")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gender of the client.
        /// </summary>
        [Display(Description = "Gender of the client")]
        [DefaultValue("M")]
        public string Gender { get; set; }

        /// <summary>
        /// Declared income of the client.
        /// </summary>
        [Display(Description = "Declared income of the client")]
        [DefaultValue(52000.80)]
        public decimal Income { get; set; }
    }
}
