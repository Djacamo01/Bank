using System;
using System.Collections.Generic;
using System.ComponentModel; // For Description attribute
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lafise.API.controllers
{
    /// <summary>
    /// Controller for managing account operations.
    /// </summary>
    /// <remarks>
    /// This controller handles all account-related operations including:
    /// - Retrieving accounts
    /// - Account management
    /// - Related account information requests
    /// </remarks>
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Gets all accounts.
        /// </summary>
        /// <returns>Returns a list of all account details.</returns>
        /// <remarks>
        /// Obtains the complete list of accounts in the system.
        /// </remarks>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AccountDetailsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Description("Gets all accounts")]
        public async Task<ActionResult<List<AccountDetailsDto>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                // In a real-world scenario you might wrap in an error response DTO.
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}