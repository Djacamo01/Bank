using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    /// <summary>
    /// Represents a bank account entity.
    /// </summary>
    /// <remarks>
    /// The Account entity contains:
    /// - Unique identifier and account metadata
    /// - Account type and balance
    /// - Creation/modification timestamps
    /// - Ownership reference to the client
    /// - Navigation to related transactions
    /// </remarks>
    public class Account : IEntity, IAuditable
    {
        /// <summary>
        /// Unique identifier of the authenticated user.
        /// </summary>
        [Display(Description = "Unique identifier of the authenticated user")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public string Id { get; set; }

        /// <summary>
        /// Unique account number assigned to the account.
        /// </summary>
        [Display(Description = "Unique account number")]
        public required string AccountNumber { get; set; }

        /// <summary>
        /// Type of the account (e.g., Savings, Checking).
        /// </summary>
        [Display(Description = "Type of the account, such as Savings or Checking")]
        public required string AccountType { get; set; }

        /// <summary>
        /// Current balance of the account.
        /// </summary>
        [Display(Description = "Current balance of the account")]
        public decimal CurrentBalance { get; set; } = 0.00m;

        /// <summary>
        /// Date and time when the account was created.
        /// </summary>
        [Display(Description = "Date and time when the account was created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time when the account was last modified, if applicable.
        /// </summary>
        [Display(Description = "Date and time when the account was last modified")]
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Foreign key referencing the owner (Client).
        /// </summary>
        [Display(Description = "Foreign key referencing the owner (Client)")]
        public required string ClientId { get; set; }

        /// <summary>
        /// Navigation property referencing the owner client.
        /// </summary>
        public Client? Client { get; set; }

        /// <summary>
        /// Navigation collection for transactions related to this account.
        /// </summary>
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}