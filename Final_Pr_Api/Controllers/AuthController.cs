﻿using Final_Pr_Api.Models;
using Final_Pr_Api.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


namespace Final_Pr_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Auth auth)
        {

            var result = _authService.Authenticate(auth.email, auth.password);

            if (result.Token != null)
            {
                return Ok(new { Token = result.Token });
            }
            else
            {
                return Unauthorized("Credenciales inválidas.");
            }
        }

        [HttpPost("verifyToken")]
        public IActionResult VerifyToken([FromBody] string token)
        {
            if (JwtService.ValidateToken(token))
            {
                return Ok(new { message = "Token valido" });
            }
            else
            {
                return Unauthorized(new { message = "Token inválido o expirado" });
            }
        }
    }
}
