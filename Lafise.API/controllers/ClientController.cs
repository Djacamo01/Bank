using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.controllers.Dto;
using Lafise.API.data.model;
using Lafise.API.services.clients;
using Lafise.API.services.Clients.Dto;
using Lafise.API.utils;
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
        /// Gets all clients with pagination.
        /// </summary>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 10, max: 100)</param>
        /// <returns>Returns a paginated list of all client details.</returns>
        /// <remarks>
        /// Obtains a paginated list of clients in the system.
        /// </remarks>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedDto<ClientResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
        [Description("Gets all clients with pagination")]
        public async Task<ActionResult<PagedDto<ClientResponseDto>>> GetAllClients(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagination = new PaginationRequestDto { Page = page, PageSize = pageSize };
                var clients = await _clientService.GetAllClients(pagination);
                return Ok(clients);
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
                    Message = "An error occurred while retrieving clients.", 
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