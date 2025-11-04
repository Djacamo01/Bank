using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    /// <summary>
    /// Represents a transaction entity in the banking system.
    /// </summary>
    /// <remarks>
    /// The Transaction entity contains:
    /// - Unique identifier for the transaction
    /// - Account linkage via AccountId
    /// - Transaction type (e.g. Deposit, Withdrawal)
    /// - Transaction amount and resulting account balance
    /// - Timestamp of the transaction (Date)
    /// - Creation/modification timestamps for auditing
    /// - Navigation to the related Account
    /// </remarks>
    public class Transaction : IEntity, IAuditable
    {
        /// <summary>
        /// Unique identifier of the transaction.
        /// </summary>
        [Display(Description = "Unique identifier of the transaction")]
        [DefaultValue("05f3daf7-234b-4e9c-839d-d7565e3a1871")]
        public  string Id { get; set; }

        /// <summary>
        /// Foreign key referencing the associated account.
        /// </summary>
        [Display(Description = "Foreign key referencing the associated account")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public required string AccountId { get; set; }

        /// <summary>
        /// Type of the transaction (e.g., Deposit, Withdrawal).
        /// </summary>
        [Display(Description = "Type of the transaction (e.g., Deposit, Withdrawal)")]
        [DefaultValue("Deposit")]
        public required string Type { get; set; }

        /// <summary>
        /// Amount involved in the transaction.
        /// </summary>
        [Display(Description = "Amount involved in the transaction")]
        [DefaultValue(200.00)]
        public required decimal Amount { get; set; }

        /// <summary>
        /// Date and time when the transaction occurred.
        /// </summary>
        [Display(Description = "Date and time when the transaction occurred")]
        [DefaultValue("2023-01-01T12:34:56Z")]
        public required DateTime Date { get; set; } = DateTime.Now;

        /// <summary>
        /// Account balance after the transaction.
        /// </summary>
        [Display(Description = "Account balance after the transaction")]
        [DefaultValue(1200.00)]
        public decimal BalanceAfter { get; set; }

        /// <summary>
        /// Date and time when this record was created.
        /// </summary>
        [Display(Description = "Date and time when the transaction record was created")]
        [DefaultValue("2023-01-01T12:34:56Z")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time when this record was last modified, if applicable.
        /// </summary>
        [Display(Description = "Date and time when the transaction record was last modified")]
        [DefaultValue("2023-01-02T08:15:00Z")]
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Navigation property referencing the related account.
        /// </summary>
        public Account? Account { get; set; }
    }
}