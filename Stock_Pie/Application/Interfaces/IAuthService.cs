using Stock_Pie.Application.Dto;

namespace Stock_Pie.Application.Interfaces
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken)> LoginAsync(UserLoginDto dto);
        Task<(string AccessToken, string RefreshToken)> RefreshAsync(string refreshToken);
        Task LogoutAsync(Guid userId);

        // New for OTP and Google
        Task<(string AccessToken, string RefreshToken)> LoginWithEmailAsync(string email);
        Task<(string AccessToken, string RefreshToken)> LoginWithGoogleAsync(string idToken);
    }
}