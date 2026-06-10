using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Stock_Pie.Application.Interfaces;

namespace Stock_Pie.Application.Services
{
    public class UserContext(IHttpContextAccessor http) : IUserContext
    {
        private readonly IHttpContextAccessor _http = http;

        public Guid UserId
        {
            get
            {
                var sub = _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(sub, out var id)) return id;
                return Guid.Empty;
            }
        }

        public string? Email => _http.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? _http.HttpContext?.User?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
    }
}
