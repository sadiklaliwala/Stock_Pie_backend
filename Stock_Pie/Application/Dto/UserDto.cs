using System;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Application.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public AuthProvider Provider { get; set; }
        public string? ProviderUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
    }
}