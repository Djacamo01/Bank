using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.data.model
{
    /// <summary>
    /// Represents a client entity.
    /// </summary>
    /// <remarks>
    /// The Client entity contains:
    /// - Unique identifier
    /// - Personal information and contact data
    /// - Authentication (password) data
    /// - Demographic and income fields
    /// - Timestamps for record creation/modification
    /// - Navigation to accounts
    /// </remarks>
    public class Client : IEntity, IAuditable
    {
        /// <summary>
        /// Unique identifier of the client.
        /// </summary>
        [Display(Description = "Unique identifier for the client")]
        [DefaultValue("51b9ecc1-f701-4f67-8554-73ff0db565fc")]
        public string Id { get; set; }

        /// <summary>
        /// Client's first name.
        /// </summary>
        [Display(Description = "Client's first name")]
        [DefaultValue("Juan")]
        public required string Name { get; set; }

        /// <summary>
        /// Tax identification number (RUC/Cédula/NIT/Pasaporte) of the client.
        /// </summary>
        [Display(Description = "Tax identification number of the client")]
        [DefaultValue("001-231019-0007E")]
        public required string TaxId { get; set; }

        /// <summary>
        /// Client's last name.
        /// </summary>
        [Display(Description = "Client's last name")]
        [DefaultValue("Perez")]
        public required string LastName { get; set; }

        /// <summary>
        /// Email address of the client.
        /// </summary>
        [Display(Description = "Email address of the client")]
        [DefaultValue("juan.perez@email.com")]
        public string? Email { get; set; }

        /// <summary>
        /// Hash of the client's password.
        /// </summary>
        [Display(Description = "Hashed password for authentication")]
        public string PasswordHash { get; set; }

        /// <summary>
        /// Salt used for hashing the client's password.
        /// </summary>
        [Display(Description = "Salt value used in hashing the password")]
        public string PasswordSalt { get; set; }

        /// <summary>
        /// Client's date of birth.
        /// </summary>
        [Display(Description = "Client's date of birth")]
        [DefaultValue(typeof(DateTime), "1991-10-23")]
        public required DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gender of the client.
        /// </summary>
        [Display(Description = "Gender of the client")]
        [DefaultValue("M")]
        public required string Gender { get; set; }

        /// <summary>
        /// Declared income of the client.
        /// </summary>
        [Display(Description = "Declared income of the client")]
        [DefaultValue(52000.80)]
        public required decimal Income { get; set; }

        /// <summary>
        /// Date and time when the client record was created.
        /// </summary>
        [Display(Description = "Date and time when the client was created")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time when the client record was last modified.
        /// </summary>
        [Display(Description = "Date and time when the client was last modified")]
        public DateTime? DateModified { get; set; }

        /// <summary>
        /// Navigation collection for accounts belonging to this client.
        /// </summary>
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
