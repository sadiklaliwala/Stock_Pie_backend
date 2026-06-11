using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stock_Pie.Domain.Entities
{
    public enum AuthProvider
    {
        Local,
        Google,
        GitHub
    }

    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? FullName { get; set; }

        // For local users - nullable for OAuth users
        public string? PasswordHash { get; set; }

        public AuthProvider Provider { get; set; } = AuthProvider.Local;

        // For OAuth users
        public string? ProviderUserId { get; set; }

        // Stored as hash (SHA256) for security. Plain refresh token is never persisted.
        public string? RefreshTokenHash { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Email OTP fields
        public string? EmailOtpHash { get; set; }
        public DateTime? EmailOtpExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public List<Transaction> Transactions { get; set; } = new();
        public List<Portfolio> Portfolios { get; set; } = new();
        public List<Asset> Assets { get; set; } = new();
        public List<Withdrawal> Withdrawals { get; set; } = new();

        public WatchList? WatchList { get; set; }

        // One-to-one wallet
        public Wallet? Wallet { get; set; }

        public ICollection<PaymentOrder> PaymentOrders { get; set; } = [];

        // Minimal bank account storage: store a one-way hash and last4 for display
        // Note: this does NOT verify ownership and should not be used for payouts.
        public string? BankAccountHash { get; set; }
        public string? BankAccountLast4 { get; set; }
    }
}