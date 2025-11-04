using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.services.Auth.Dto;

namespace Lafise.API.services.Auth
{
    public interface IAuthService
    {
        Task<LoginResultDto> Login(string userEmail, string password);
    }
}

