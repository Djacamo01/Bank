using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.services.Transactions;
using Lafise.API.services.Transactions.Dto;
using Lafise.API.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lafise.API.controllers
{
    /// <summary>
    /// Controller for managing transaction operations.
    /// </summary>
    /// <remarks>
    /// This controller handles all transaction-related operations including:
    /// - Deposits (only to own accounts)
    /// - Withdrawals (only from own accounts)
    /// - Transfers (from own accounts to any account)
    /// - Transaction history
    /// </remarks>
    [ApiController]
    [Route("api/transactions")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
        }

        /// <summary>
        /// Creates a deposit transaction for the specified account.
        /// </summary>
        /// <param name="request">The deposit request containing account number and amount.</param>
        /// <returns>The created transaction details.</returns>
        /// <remarks>
        /// Validates that the account belongs to the authenticated user. Increments the account balance by the specified amount and creates a transaction record.
        /// </remarks>
        [HttpPost("deposit")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Creates a deposit transaction")]
        public async Task<ActionResult<TransactionDto>> Deposit([FromBody] CreateTransactionDto request)
        {
            try
            {
                var transaction = await _transactionService.Deposit(request);
                return Ok(transaction);
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
                    Message = "An error occurred while processing the deposit.", 
                    Detail = ex.Message 
                });
            }
        }

        /// <summary>
        /// Creates a withdrawal transaction for the specified account.
        /// </summary>
        /// <param name="request">The withdrawal request containing account number and amount.</param>
        /// <returns>The created transaction details.</returns>
        /// <remarks>
        /// Validates that the account belongs to the authenticated user and has sufficient balance before processing the withdrawal.
        /// If insufficient funds or the account doesn't belong to the user, the transaction is rejected.
        /// </remarks>
        [HttpPost("withdraw")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Creates a withdrawal transaction")]
        public async Task<ActionResult<TransactionDto>> Withdraw([FromBody] CreateTransactionDto request)
        {
            try
            {
                var transaction = await _transactionService.Withdraw(request);
                return Ok(transaction);
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
                    Message = "An error occurred while processing the withdrawal.", 
                    Detail = ex.Message 
                });
            }
        }

        /// <summary>
        /// Transfers funds from one account to another account.
        /// </summary>
        /// <param name="request">The transfer request containing source account, destination account, and amount.</param>
        /// <returns>The withdrawal transaction details from the source account.</returns>
        /// <remarks>
        /// Validates that the source account belongs to the authenticated user and has sufficient balance.
        /// The destination account can belong to any user. Creates two transactions: one for the source account (Transfer Out)
        /// and one for the destination account (Transfer In).
        /// </remarks>
        [HttpPost("transfer")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Transfers funds between accounts")]
        public async Task<ActionResult<TransactionDto>> Transfer([FromBody] TransferDto request)
        {
            try
            {
                var transaction = await _transactionService.Transfer(request);
                return Ok(transaction);
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
                    Message = "An error occurred while processing the transfer.", 
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
        [HttpGet("movements/{accountNumber}")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets all transactions for an account")]
        public async Task<ActionResult<System.Collections.Generic.List<TransactionDto>>> GetAccountMovements(string accountNumber)
        {
            try
            {
                var movements = await _transactionService.GetAccountMovements(accountNumber);
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

