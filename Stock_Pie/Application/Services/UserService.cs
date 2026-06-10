using Microsoft.EntityFrameworkCore;
using Stock_Pie.Application.Dto;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Domain.Entities;
using Stock_Pie.Infrastructure.Persistence;
using AutoMapper;

namespace Stock_Pie.Application.Services
{
    public class UserService(
        IUserRepository userRepo,
        IWatchlistService watchlistService,
        IMapper mapper,
        IWalletService walletService,
        AppDbContext db) : IUserService
    {
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IWalletService _walletService = walletService;
        private readonly IWatchlistService _watchlistService = watchlistService;
        private readonly AppDbContext _db = db;

        public async Task<User> CreateUserAsync(UserRegisterDto dto)
        {
            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                throw new InvalidOperationException("User with email already exists");

            // wrap in transaction — if wallet or watchlist creation fails, user is rolled back too
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Provider = AuthProvider.Local,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                await _watchlistService.CreateWatchList(user.Id);
                await _walletService.CreateWalletForUserAsync(user.Id);

                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepo.GetByEmailAsync(email);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _userRepo.GetByIdAsync(id);
        }

        public async Task<User?> UpdateUserAsync(Guid id, UserRegisterDto dto)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return null;

            user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            await _userRepo.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepo.GetByIdAsync(id);
            if (user == null) return false;

            await _userRepo.RemoveAsync(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }

        public async Task<User?> AuthenticateAsync(UserLoginDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null) return null;
            if (user.PasswordHash == null) return null;

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) return null;

            user.LastLoginAt = DateTime.UtcNow;
            await _userRepo.SaveChangesAsync();
            return user;
        }
    }
}