using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.services.clients;
using Microsoft.AspNetCore.Mvc;

namespace Lafise.API.controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController:ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _clientService.GetAllClients();
            if (clients == null)
            {
                return NotFound();
            }
            return Ok(clients);
        }

    }
}