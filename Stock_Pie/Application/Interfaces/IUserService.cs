using Stock_Pie.Application.Dto;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(UserRegisterDto dto);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> UpdateUserAsync(Guid id, UserUpdateDto dto);
        Task<bool> DeleteUserAsync(Guid id);
        Task<User?> AuthenticateAsync(UserLoginDto dto);
        
    }
}