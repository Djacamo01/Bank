using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lafise.API.data.model;
using Lafise.API.services.Auth.Dto;

namespace Lafise.API.services.Auth.JWT
{
    public interface IJwtTokenGenerator
    {
        TokenInfo CreateToken(Client user);
        string GenerateRefreshToken(Client user);
    }
}