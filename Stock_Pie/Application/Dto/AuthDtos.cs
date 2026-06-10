namespace Stock_Pie.Application.Dto
{
    public class AuthLoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = null!;
    }
}
