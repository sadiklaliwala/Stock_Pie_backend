using System;
using System.Security.Cryptography;
using System.Text;

namespace Stock_Pie.Application.Services
{
    public interface IRefreshTokenService
    {
        (string tokenPlain, string tokenHash) GenerateRefreshToken();
        string HashToken(string tokenPlain);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        public (string tokenPlain, string tokenHash) GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            var token = Convert.ToBase64String(bytes);
            var hash = HashToken(token);
            return (token, hash);
        }

        public string HashToken(string tokenPlain)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlain));
            return Convert.ToHexString(bytes);
        }
    }
}