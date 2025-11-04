using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Lafise.API.services.Auth.Dto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Lafise.API.services.Auth.JWT
{
    public class JwtTokenGenerator:IJwtTokenGenerator
    {
        private readonly IConfiguration _config;
        private const int TOKEN_EXPIRATION_MINUTES = 60; // Expiración fija en 60 minutos

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public TokenInfo CreateToken(Client user, string accountNumber )
        {
            var claims = new List<Claim>()
            {
                new Claim("UserId", user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.LastName}"),
                new Claim("AccountNumber", accountNumber),
            };

            var key = _config.GetValue<string>("jwt-token-secret-key");
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("JWT token secret key is not configured.");

            var issuer = _config.GetValue<string>("jwt-issuer");
            var audience = _config.GetValue<string>("jwt-audience");

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.UtcNow.AddMinutes(TOKEN_EXPIRATION_MINUTES),
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenInfo
            {
                Token = tokenHandler.WriteToken(token),
                ExpirationDate = tokenDescriptor.Expires.Value
            };
        }



        public string GenerateRefreshToken(Client user)
        {
            var randomNumber = new byte[32];
            using (var generator = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);

                user.RefreshToken = token;
                user.RefreshTokenExpiration = System.DateTime.UtcNow.AddMinutes(TOKEN_EXPIRATION_MINUTES);                

                return token;
            }
        }
    }
}