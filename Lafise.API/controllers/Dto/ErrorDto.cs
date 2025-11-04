using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.controllers.Dto
{
    /// <summary>
    /// Standard error response DTO for API controllers.
    /// </summary>
    public class ErrorDto
    {
        /// <summary>
        /// Indicates that an error occurred. Always true for error responses.
        /// </summary>
        [Display(Description = "Error indicator")]
        [DefaultValue(true)]
        public bool Error { get; set; } = true;

        /// <summary>
        /// HTTP status code of the error.
        /// </summary>
        [Display(Description = "HTTP status code")]
        [DefaultValue(400)]
        public int Code { get; set; }

        /// <summary>
        /// Error message describing what went wrong.
        /// </summary>
        [Display(Description = "Error message")]
        [DefaultValue("An error occurred")]
        public required string Message { get; set; }

        /// <summary>
        /// Optional detailed error information. Typically used for internal server errors.
        /// </summary>
        [Display(Description = "Detailed error information")]
        public string? Detail { get; set; }
    }
}

