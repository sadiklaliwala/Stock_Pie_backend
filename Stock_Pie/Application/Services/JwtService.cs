using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Stock_Pie.Application.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(Guid userId, string email);
        int AccessTokenExpiryMinutes { get; }
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;
        public int AccessTokenExpiryMinutes { get; }

        public JwtService(IConfiguration config)
        {
            _config = config;
            var secret = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            _issuer = _config["Jwt:Issuer"] ?? "stock_pie";
            _audience = _config["Jwt:Audience"] ?? "stock_pie_audience";
            AccessTokenExpiryMinutes = int.TryParse(_config["Jwt:AccessTokenMinutes"], out var m) ? m : 15;
        }

        public string GenerateAccessToken(Guid userId, string email)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}