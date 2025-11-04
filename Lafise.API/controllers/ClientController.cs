using System;
using System.Collections.Generic;
using System.ComponentModel; 
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Lafise.API.services.clients;
using Lafise.API.services.Clients.Dto;
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
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        [Description("Gets all clients")]
        public async Task<ActionResult<List<Client>>> GetAllClients()
        {
            try
            {
                var clients = await _clientService.GetAllClients();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                // In a real-world scenario you might wrap in an error response DTO.
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
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
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Description("Creates a new client")]
        public async Task<ActionResult<ClientResponseDto>> CreateClient([FromBody] CreateClientDto client)
        {
            try
            {
                var newClient = await _clientService.CreateClient(client);
                return Ok(newClient);
            }
            catch (Exception ex)
            {
                // In a real-world scenario you might validate, check for duplicates, etc.
                return BadRequest(ex.Message);
            }
        }
    }
}