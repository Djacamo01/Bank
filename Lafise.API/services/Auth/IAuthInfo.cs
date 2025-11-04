using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lafise.API.services.Auth
{
    public interface IAuthInfo
    {
        string UserId();

        string UserName();

        string UserEmail();

    }
}
