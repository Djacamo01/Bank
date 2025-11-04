using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Clients.Dto
{
    /// <summary>
    /// Data Transfer Object for returning detailed client information.
    /// </summary>
    /// <remarks>
    /// Provides identity, contact, personal and audit information for a client.
    /// </remarks>
    public class ClientResponseDto
    {
        /// <summary>
        /// Unique identifier for the client.
        /// </summary>
        [Display(Description = "Unique identifier of the client")]
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

        /// <summary>
        /// Account number for the client.
        /// </summary>
        [Display(Description = "Account number for the client")]
        [DefaultValue("10001111222")]
        public string AccountNumber { get; set; }

        /// <summary>
        /// Date and time the client record was created.
        /// </summary>
        [Display(Description = "Date and time when the client record was created")]
        [DefaultValue("2023-01-01T08:30:00Z")]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Date and time the client record was last modified, if any.
        /// </summary>
        [Display(Description = "Date and time when the client record was last modified")]
        [DefaultValue("2023-02-01T09:30:00Z")]
        public DateTime? DateModified { get; set; }
    }
}
