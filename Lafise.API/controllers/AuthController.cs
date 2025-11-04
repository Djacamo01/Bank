using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.services.Auth;
using Lafise.API.services.Auth.Dto;
using Lafise.API.utils;
using Microsoft.AspNetCore.Mvc;

namespace Lafise.API.controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(LoginResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto.Email, loginDto.Password);
                return Ok(result);
            }
            catch (LafiseException ex)
            {
                return StatusCode(ex.Code, new { error = true, code = ex.Code, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = true, code = 500, message = "An error occurred during login.", detail = ex.Message });
            }
        }
    }
}