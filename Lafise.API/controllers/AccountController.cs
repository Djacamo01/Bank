using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Transactions.Dto;
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
        [ProducesResponseType(typeof(List<AccountDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets all accounts")]
        public async Task<ActionResult<List<AccountDto>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts();
                return Ok(accounts);
            }
            catch (LafiseException ex)
            {
                return StatusCode(ex.Code, new ErrorDto { Code = ex.Code, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDto 
                { 
                    Code = 500, 
                    Message = "An error occurred while retrieving accounts.", 
                    Detail = ex.Message 
                });
            }
        }


        /// <summary>
        /// Creates a new account.
        /// </summary>
        /// <param name="accountType">The type of account to create.</param>
        /// <returns>The created account's details.</returns>
        /// <remarks>
        /// Adds a new account to the system.
        /// </remarks>
        [HttpPost("create")]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Creates a new account")]
        public async Task<ActionResult<AccountDto>> CreateAccount([FromQuery] string accountType)
        {
            try
            {
                var account = await _accountService.CreateAccount(accountType);
                return Ok(account);
            }
            catch (LafiseException ex)
            {
                return StatusCode(ex.Code, new ErrorDto { Code = ex.Code, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDto 
                { 
                    Code = 500, 
                    Message = "An error occurred while creating the account.", 
                    Detail = ex.Message 
                });
            }
        }

        /// <summary>
        /// Gets all transactions (movements) for a specific account.
        /// </summary>
        /// <param name="accountNumber">The account number to get transactions for.</param>
        /// <returns>List of transactions for the account.</returns>
        /// <remarks>
        /// Returns all transactions for the specified account, ordered by date (most recent first).
        /// </remarks>
        [HttpGet("{accountNumber}/movements")]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets all transactions (movements) for an account")]
        public async Task<ActionResult<List<TransactionDto>>> GetAccountMovements(string accountNumber)
        {
            try
            {
                var movements = await _accountService.GetAccountMovements(accountNumber);
                return Ok(movements);
            }
            catch (LafiseException ex)
            {
                return StatusCode(ex.Code, new ErrorDto { Code = ex.Code, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorDto 
                { 
                    Code = 500, 
                    Message = "An error occurred while retrieving account movements.", 
                    Detail = ex.Message 
                });
            }
        }
            
    }
}
    