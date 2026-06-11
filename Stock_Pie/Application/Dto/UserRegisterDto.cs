using System.ComponentModel.DataAnnotations;

namespace Stock_Pie.Application.Dto
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = null!;

        // Optional bank account string (free-form, stored as hash + last4)
        public string? BankAccount { get; set; }
    }

    public class UserUpdateDto
    {
        [EmailAddress]
        public string? Email { get; set; } = null!;

        public string? FullName { get; set; } = null!;

        [MinLength(6)]
        public string? Password { get; set; } = null!;

        // Optional bank account string (free-form, stored as hash + last4)
        public string? BankAccount { get; set; }
    }
}