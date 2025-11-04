using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.services.Accounts;
using Lafise.API.services.Accounts.Dto;
using Lafise.API.services.Transactions.Dto;
using TransactionSummaryDto = Lafise.API.services.Transactions.Dto.TransactionSummaryDto;
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
        /// Gets the current balance of a specific account by account number.
        /// </summary>
        /// <param name="accountNumber">The account number to query the balance for.</param>
        /// <returns>The current balance information for the account.</returns>
        /// <remarks>
        /// Returns the current balance, account type, and last updated timestamp for the specified account.
        /// </remarks>
        [HttpGet("{accountNumber}/balance")]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AccountBalanceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets the current balance of an account by account number")]
        public async Task<ActionResult<AccountBalanceDto>> GetAccountBalance(string accountNumber)
        {
            try
            {
                var balance = await _accountService.GetAccountBalance(accountNumber);
                return Ok(balance);
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
                    Message = "An error occurred while retrieving account balance.", 
                    Detail = ex.Message 
                });
            }
        }

        /// <summary>
        /// Gets all transactions (movements) for a specific account with pagination.
        /// </summary>
        /// <param name="accountNumber">The account number to get transactions for.</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <returns>Paginated list of transactions for the account.</returns>
        /// <remarks>
        /// Returns a paginated list of transactions for the specified account, ordered by date (most recent first).
        /// </remarks>
        [HttpGet("{accountNumber}/movements")]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedDtoSummary<TransactionDto, TransactionSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets all transactions (movements) for an account with pagination")]
        public async Task<ActionResult<PagedDtoSummary<TransactionDto, TransactionSummaryDto>>> GetAccountMovements(
            string accountNumber,
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagination = new PaginationRequestDto { Page = page, PageSize = pageSize };
                var movements = await _accountService.GetAccountMovements(accountNumber, pagination);
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
    