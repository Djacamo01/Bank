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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto.Email, loginDto.Password);
                return Ok(result);
            }
            catch (LafiseException ex)
            {
                return StatusCode(ex.Code, new { message = ex.Message, code = ex.Code });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }
    }
}