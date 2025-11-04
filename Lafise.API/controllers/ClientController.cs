using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.data.model;
using Lafise.API.services.clients;
using Lafise.API.services.Clients;
using Lafise.API.services.Clients.Dto;
using Lafise.API.utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lafise.API.controllers
{
    /// <summary>
    /// Controller for managing client operations.
    /// </summary>
    /// <remarks>
    /// This controller handles all client-related operations including:
    /// - Retrieving clients
    /// - Client management
    /// </remarks>
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// Gets a comprehensive summary of all accounts and their transaction statistics for the authenticated client.
        /// </summary>
        /// <returns>Summary containing all accounts, balances, and transaction statistics.</returns>
        /// <remarks>
        /// Returns a detailed summary including:
        /// - Client information (ID, name, email)
        /// - List of all accounts with their details
        /// - Transaction summaries per account (deposits, withdrawals, transfers)
        /// - Total balance across all accounts
        /// - Total number of accounts
        /// </remarks>
        [HttpGet("summary")]
        [Authorize]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClientAccountsSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets comprehensive summary of all accounts and transactions for the authenticated client")]
        public async Task<ActionResult<ClientAccountsSummaryDto>> GetClientAccountsSummary()
        {
            try
            {
                var summary = await _clientService.GetClientAccountsSummary();
                return Ok(summary);
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
                    Message = "An error occurred while retrieving client accounts summary.", 
                    Detail = ex.Message 
                });
            }
        }

        

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="client">Client creation data.</param>
        /// <returns>The created client's details.</returns>
        /// <remarks>
        /// Adds a new client to the system.
        /// </remarks>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Creates a new client")]
        public async Task<ActionResult<ClientResponseDto>> CreateClient([FromBody] CreateClientDto client)
        {
            try
            {
                var newClient = await _clientService.CreateClient(client);
                return Ok(newClient);
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
                    Message = "An error occurred while creating the client.", 
                    Detail = ex.Message 
                });
            }
        }
    }
}