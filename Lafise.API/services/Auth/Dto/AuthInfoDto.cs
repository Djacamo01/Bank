using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Auth.Dto
{
    public class AuthInfoDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string AccountNumber { get; set; }
    }
}