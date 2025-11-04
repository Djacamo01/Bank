using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Clients.Dto
{
    /// <summary>
    /// Data Transfer Object for creating a new client.
    /// </summary>
    /// <remarks>
    /// Captures personal data, credentials, and initial account type information for client creation.
    /// </remarks>
    public class CreateClientDto
    {
        /// <summary>
        /// Client's first name.
        /// </summary>
        [Display(Description = "Client's first name")]
        [DefaultValue("Juan")]
        public required string Name { get; set; }

        /// <summary>
        /// Client's last name.
        /// </summary>
        [Display(Description = "Client's last name")]
        [DefaultValue("Perez")]
        public required string LastName { get; set; }

        /// <summary>
        /// Client's tax identification number.
        /// </summary>
        [Display(Description = "Client's tax identification number")]
        [DefaultValue("A1B2C3-XYZ")]
        public required string TaxId { get; set; }

        /// <summary>
        /// Email address of the client (optional).
        /// </summary>
        [Display(Description = "Email address of the client")]
        [DefaultValue("juan.perez@email.com")]
        public string? Email { get; set; }

        /// <summary>
        /// Desired password for the client account.
        /// </summary>
        [Display(Description = "Password for the client account")]
        [DefaultValue("StrongPassword123!")]
        public required string Password { get; set; }

        /// <summary>
        /// Date of birth of the client.
        /// </summary>
        [Display(Description = "Client's date of birth")]
        [DefaultValue("1991-10-23")]
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
        /// Desired account type to create for the client (e.g., Savings, Checking).
        /// </summary>
        [Display(Description = "Desired account type to create for the client")]
        [DefaultValue("Savings")]
        public required string AccountType { get; set; }
    }
}