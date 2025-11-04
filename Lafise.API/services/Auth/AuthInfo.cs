using System.Security.Claims;

namespace Lafise.API.services.Auth
{
    public class AuthInfo : IAuthInfo
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthInfo(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId()
        {
            
            return _httpContextAccessor.HttpContext!.User.FindFirstValue("UserId") ;
                
        }

        public string UserName()
        {
            return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Name);
        }

        public string UserEmail()
        {
            return _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email);
        }

        public string AccountNumber()
        {
            return _httpContextAccessor.HttpContext!.User.FindFirstValue("AccountNumber");
        }
    }
}