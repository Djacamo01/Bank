using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data;
using Lafise.API.data.model;
using Lafise.API.services.Auth.Dto;
using Lafise.API.services.Auth.JWT;
using Lafise.API.utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Lafise.API.services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IDbContextFactory<BankDataContext> _db;
        private readonly ICryptor _cryptor;
        private readonly IConfiguration _config;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IAuthInfo _authInfo;

        public AuthService(IDbContextFactory<BankDataContext> db, ICryptor cryptor, IConfiguration config, IJwtTokenGenerator jwtTokenGenerator, IAuthInfo authInfo)
        {
            _db = db;
            _cryptor = cryptor;
            _config = config;
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
            _authInfo = authInfo ?? throw new ArgumentNullException(nameof(authInfo));
        }

        public async Task<LoginResultDto> Login(string userEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                throw new LafiseException(400, "Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(password))
                throw new LafiseException(400, "Password cannot be empty.");

            using (var context = await _db.CreateDbContextAsync())
            {
                // Buscar el usuario por email e incluir sus cuentas
                var user = await context.Clients
                    .Include(c => c.Accounts)
                    .FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                    throw new LafiseException(404, "User not found.");

                // Validar el password
                if (!_cryptor.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                    throw new LafiseException(401, "Invalid password.");

                // Obtener el número de cuenta (primera cuenta del cliente)
                var accountNumber = user.Accounts?.FirstOrDefault()?.AccountNumber ?? string.Empty;

                // Generar token y refresh token
                var token = _jwtTokenGenerator.CreateToken(user, accountNumber);
                var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user);

                // Guardar el refresh token en la base de datos
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiration = token.ExpirationDate;

                context.AddOrUpdate(user);
                await context.SaveChangesAsync();

                // Construir el login result
                var loginResult = new LoginResultDto
                {
                    UserId = user.Id,
                    UserName = $"{user.Name} {user.LastName}",
                    UserEmail = user.Email ?? string.Empty,
                    AuthToken = token.Token,
                    RefreshToken = refreshToken,
                    AuthTokenExpiration = token.ExpirationDate
                };

                return loginResult;
            }
        }


        public Task<AuthInfoDto> GetAuthInfo()
        {
            var authInfoDto = new AuthInfoDto
            {
                UserId = _authInfo.UserId(),
                UserName = _authInfo.UserName(),
                UserEmail = _authInfo.UserEmail(),
                AccountNumber = _authInfo.AccountNumber()
            };
            return Task.FromResult(authInfoDto);
        }

    }
}