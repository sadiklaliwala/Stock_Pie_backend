using System.ComponentModel.DataAnnotations;

namespace Stock_Pie.Application.Dto
{
    public class SendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Otp { get; set; } = null!;
    }

    public class GoogleAuthRequest
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
