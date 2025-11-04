using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Linq;
using System.Threading.Tasks;
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
        /// Gets all clients.
        /// </summary>
        /// <returns>Returns a list of all client details.</returns>
        /// <remarks>
        /// Obtains the complete list of clients in the system.
        /// </remarks>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<Client>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [Description("Gets all clients")]
        public async Task<ActionResult<List<Client>>> GetAllClients()
        {
            try
            {
                var clients = await _clientService.GetAllClients();
                return Ok(clients);
            }
            catch (LafiseException ex)
            {
                return StatusCode(ex.Code, new { error = true, code = ex.Code, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, code = 500, message = "An error occurred while retrieving clients.", detail = ex.Message });
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
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
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
                return StatusCode(ex.Code, new { error = true, code = ex.Code, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, code = 500, message = "An error occurred while creating the client.", detail = ex.Message });
            }
        }
    }
}