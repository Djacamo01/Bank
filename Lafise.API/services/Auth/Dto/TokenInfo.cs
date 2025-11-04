using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Auth.Dto
{
    public class TokenInfo
    {
        public string Token { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}