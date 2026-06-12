using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;
using Google.Apis.Auth;
using Stock_Pie.Application.Interfaces;

namespace Stock_Pie.Application.Services
{

    public class AuthService(AppDbContext db, IJwtService jwt, IRefreshTokenService rt, IConfiguration config , IWalletService walletService , IWatchlistService watchlistService) : IAuthService
    {
        private readonly AppDbContext _db = db;
        private readonly IJwtService _jwt = jwt;
        private readonly IRefreshTokenService _rt = rt;
        private readonly IConfiguration _config = config;
                private readonly IWatchlistService _watchlistService  = watchlistService;
        private readonly IWalletService _walletService= walletService;


        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(UserLoginDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == dto.Email)
    ?? throw new UnauthorizedAccessException("Invalid email or password.");

            if (user.PasswordHash == null) throw new NotSupportedException("This account uses social login.");

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) throw new UnauthorizedAccessException("Invalid email or password.");

            var access = _jwt.GenerateAccessToken(user.Id, user.Email);
            var (rtPlain, rtHash) = _rt.GenerateRefreshToken();
            user.RefreshTokenHash = rtHash;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            return (access, rtPlain);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken)
        {
            var hash = _rt.HashToken(refreshToken);
            var user = await _db.Users.SingleOrDefaultAsync(u => u.RefreshTokenHash == hash)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (!user.RefreshTokenExpiryTime.HasValue || user.RefreshTokenExpiryTime.Value < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired.");
            // Rotate
            var access = _jwt.GenerateAccessToken(user.Id, user.Email);
            var (newPlain, newHash) = _rt.GenerateRefreshToken();
            user.RefreshTokenHash = newHash;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            await _db.SaveChangesAsync();

            return (access, newPlain);
        }

        public async Task LogoutAsync(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return;
            user.RefreshTokenHash = null;
            user.RefreshTokenExpiryTime = null;
            await _db.SaveChangesAsync();
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginWithEmailAsync(string email)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == email)
     ?? throw new KeyNotFoundException("User not found.");
            var access = _jwt.GenerateAccessToken(user.Id, user.Email);
            var (rtPlain, rtHash) = _rt.GenerateRefreshToken();
            user.RefreshTokenHash = rtHash;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            //await _watchlistService.CreateWatchList(user.Id);
            //await _walletService.CreateWalletForUserAsync(user.Id);
            return (access, rtPlain);
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginWithGoogleAsync(string idToken)
        {
            var isNewUser = false;
            var clientId = _config["Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
                throw new InvalidOperationException("Google:ClientId is not configured."); // config issue, not a missing key

            var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = [clientId] };

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Invalid Google token.", ex); // auth failure → 401
            }

            var email = payload.Email;
            var sub = payload.Subject; // google user id

            var user = await _db.Users.SingleOrDefaultAsync(u => u.Provider == AuthProvider.Google && u.ProviderUserId == sub);

            if (user == null)
            {
                // check if email already exists with local account
                var existingUser = await _db.Users.SingleOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    // link google to existing account
                    existingUser.ProviderUserId = sub;
                    // keep Provider as Local or add a separate flag for linked providers
                    user = existingUser;
                }
                else
                {
                    // brand new user via google
                    user = new User
                    {
                        Email = email,
                        FullName = payload.Name,
                        Provider = AuthProvider.Google,
                        ProviderUserId = sub,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    await _db.Users.AddAsync(user);
                    isNewUser = true;
                }
            }

            var access = _jwt.GenerateAccessToken(user.Id, user.Email);
            var (rtPlain, rtHash) = _rt.GenerateRefreshToken();
            user.RefreshTokenHash = rtHash;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
            user.LastLoginAt = DateTime.UtcNow;
             // check before SaveChangesAsync
            await _db.SaveChangesAsync();

            if (isNewUser)
            {
                await _watchlistService.CreateWatchList(user.Id);
                await _walletService.CreateWalletForUserAsync(user.Id);
            }
            return (access, rtPlain);
        }
    }
}