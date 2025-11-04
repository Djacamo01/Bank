using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Lafise.API.services.Auth.Dto
{
    /// <summary>
    /// Data transfer object for refresh token request.
    /// </summary>
    public class RefreshTokenDto
    {
        /// <summary>
        /// Refresh token to validate and use for generating a new access token.
        /// </summary>
        [Required]
        [Display(Description = "Refresh token")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public required string RefreshToken { get; set; }
    }
}

