using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Services
{
    public interface IOtpService
    {
        Task SendOtpAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otp);
    }

    public class OtpService(IConfiguration config, IEmailService email, AppDbContext db) : IOtpService
    {
        private readonly IConfiguration _config = config;
        private readonly IEmailService _email = email;
        private readonly AppDbContext _db = db;

        private static string Hash(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        public async Task SendOtpAsync(string email)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email) ?? throw new InvalidOperationException("User not found");

            // generate 6-digit OTP
            var rng = RandomNumberGenerator.GetInt32(0, 1000000);
            var otp = rng.ToString("D6");
            var hash = Hash(otp);
            user.EmailOtpHash = hash;
            user.EmailOtpExpiry = DateTime.UtcNow.AddMinutes(10);
            await _db.SaveChangesAsync();



            var subject = "OTP Verification Code";

            var body = $@"
                <h2>Your OTP Code</h2>
                <p>Your verification code is:</p>
                <h1>{otp}</h1>
                <p>This code expires in 5 minutes.</p>";
            await _email.SendOtpEmailAsync(email, subject, body);

        }

        public async Task<bool> VerifyOtpAsync(string email, string otp)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;
            if (!user.EmailOtpExpiry.HasValue || user.EmailOtpExpiry.Value < DateTime.UtcNow) return false;

            var hash = Hash(otp);
            if (!string.Equals(hash, user.EmailOtpHash, StringComparison.OrdinalIgnoreCase)) return false;

            // clear
            user.EmailOtpHash = null;
            user.EmailOtpExpiry = null;
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
