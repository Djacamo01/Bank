using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Auth.Dto
{
    /// <summary>
    /// Data transfer object for login result
    /// </summary>
    public class LoginResultDto
    {
        /// <summary>
        /// Unique identifier of the authenticated user
        /// </summary>
        [Display(Description = "Unique identifier of the authenticated user")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public string UserId { get; set; }

        /// <summary>
        /// Unique identifier of the partner organization
        /// </summary>
        [Display(Description = "Unique identifier of the partner organization")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public string UserName { get; set; }

        /// <summary>
        /// Email address of the authenticated user
        /// </summary>
        [Display(Description = "Email address of the authenticated user")]
        [DefaultValue("anakin.skywalker@traderpal.com")]
        public string UserEmail { get; set; }

        /// <summary>
        /// JWT authentication token for API access
        /// </summary>
        [Display(Description = "JWT authentication token for API access")]
        [DefaultValue("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...")]
        public string AuthToken { get; set; }

        /// <summary>
        /// Token used to obtain a new authentication token
        /// </summary>
        [Display(Description = "Token used to obtain a new authentication token")]
        [DefaultValue("2946445a-f37d-4aaa-8327-e372adf763b8")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Date and time when the authentication token expires
        /// </summary>
        [Display(Description = "Date and time when the authentication token expires")]
        [DefaultValue(typeof(DateTime), "2023-10-15T12:00:00Z")]
        public DateTime AuthTokenExpiration { get; set; }
    }
}