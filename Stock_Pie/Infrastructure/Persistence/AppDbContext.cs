using Microsoft.EntityFrameworkCore;
using Stock_Pie.Domain.Entities;

namespace Stock_Pie.Infrastructure.Persistence
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Portfolio> Portfolios { get; set; } = null!;
        public DbSet<Coin> Coins { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<WalletTransaction> WalletTransactions { get; set; } = null!;
        public DbSet<Withdrawal> Withdrawals { get; set; } = null!;
        public DbSet<Asset> Assets { get; set; } = null!;
        public DbSet<WatchList> WatchLists { get; set; } = null!;
        public DbSet<PaymentOrder> PaymentOrders { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Coin ────────────────────────────────────────────────────────
            modelBuilder.Entity<Coin>().HasData(
                new Coin
                {
                    Id = "bitcoin",
                    Symbol = "btc",
                    Name = "Bitcoin",
                    Image = "https://coin-images.coingecko.com/coins/images/1/large/bitcoin.png?1696501400",
                    CurrentPrice = 75605m,
                    MarketCap = 1512981807039,
                    MarketCapRank = 1,
                    FullyDilutedValuation = 1512981807039,
                    TotalVolume = 42502155298,
                    High24h = 76134m,
                    Low24h = 73501m,
                    PriceChange24h = 859.52m,
                    PriceChangePercentage24h = 1.14992m,
                    MarketCapChange24h = 19430849789,
                    MarketCapChangePercentage24h = 1.30098m,
                    CirculatingSupply = 20017090.0m,
                    TotalSupply = 20017090.0m,
                    MaxSupply = 21000000.0m,
                    Ath = 126080m,
                    AthChangePercentage = -40.0339m,
                    AthDate = new DateTime(2025, 10, 6, 18, 57, 42, DateTimeKind.Utc),
                    Atl = 67.81m,
                    AtlChangePercentage = 111397.2809m,
                    AtlDate = new DateTime(2013, 7, 6, 0, 0, 0, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2026, 4, 17, 12, 10, 53, DateTimeKind.Utc)
                },
                new Coin
                {
                    Id = "ethereum",
                    Symbol = "eth",
                    Name = "Ethereum",
                    Image = "https://coin-images.coingecko.com/coins/images/279/large/ethereum.png",
                    CurrentPrice = 4025m,
                    MarketCap = 483000000000,
                    MarketCapRank = 2,
                    FullyDilutedValuation = 483000000000,
                    TotalVolume = 21000000000,
                    High24h = 4100m,
                    Low24h = 3900m,
                    PriceChange24h = 85.5m,
                    PriceChangePercentage24h = 2.17m,
                    MarketCapChange24h = 9500000000,
                    MarketCapChangePercentage24h = 2.01m,
                    CirculatingSupply = 120000000.0m,
                    TotalSupply = 120000000.0m,
                    MaxSupply = null,
                    Ath = 4891m,
                    AthChangePercentage = -17.7m,
                    AthDate = new DateTime(2021, 11, 10, 14, 24, 19, DateTimeKind.Utc),
                    Atl = 0.43m,
                    AtlChangePercentage = 935000m,
                    AtlDate = new DateTime(2015, 10, 20, 0, 0, 0, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2026, 4, 17, 12, 10, 53, DateTimeKind.Utc)
                },
                new Coin
                {
                    Id = "tether",
                    Symbol = "usdt",
                    Name = "Tether",
                    Image = "https://coin-images.coingecko.com/coins/images/325/large/Tether.png",
                    CurrentPrice = 1.00m,
                    MarketCap = 102000000000,
                    MarketCapRank = 3,
                    FullyDilutedValuation = 102000000000,
                    TotalVolume = 60000000000,
                    High24h = 1.01m,
                    Low24h = 0.99m,
                    PriceChange24h = 0.001m,
                    PriceChangePercentage24h = 0.1m,
                    MarketCapChange24h = 500000000,
                    MarketCapChangePercentage24h = 0.5m,
                    CirculatingSupply = 102000000000.0m,
                    TotalSupply = 102000000000.0m,
                    MaxSupply = null,
                    Ath = 1.32m,
                    AthChangePercentage = -24.2m,
                    AthDate = new DateTime(2018, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                    Atl = 0.57m,
                    AtlChangePercentage = 75.4m,
                    AtlDate = new DateTime(2015, 3, 2, 0, 0, 0, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2026, 4, 17, 12, 10, 53, DateTimeKind.Utc)
                },
                new Coin
                {
                    Id = "binancecoin",
                    Symbol = "bnb",
                    Name = "BNB",
                    Image = "https://coin-images.coingecko.com/coins/images/825/large/bnb-icon2_2x.png",
                    CurrentPrice = 620m,
                    MarketCap = 95000000000,
                    MarketCapRank = 4,
                    FullyDilutedValuation = 95000000000,
                    TotalVolume = 1800000000,
                    High24h = 630m,
                    Low24h = 600m,
                    PriceChange24h = 12m,
                    PriceChangePercentage24h = 1.97m,
                    MarketCapChange24h = 1500000000,
                    MarketCapChangePercentage24h = 1.6m,
                    CirculatingSupply = 153000000.0m,
                    TotalSupply = 153000000.0m,
                    MaxSupply = 200000000.0m,
                    Ath = 690m,
                    AthChangePercentage = -10.1m,
                    AthDate = new DateTime(2021, 5, 10, 0, 0, 0, DateTimeKind.Utc),
                    Atl = 0.039m,
                    AtlChangePercentage = 1589000m,
                    AtlDate = new DateTime(2017, 10, 19, 0, 0, 0, DateTimeKind.Utc),
                    LastUpdated = new DateTime(2026, 4, 17, 12, 10, 53, DateTimeKind.Utc)
                }
            );

            // ── User ────────────────────────────────────────────────────────
            modelBuilder.Entity<User>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.Property(u => u.Email).IsRequired();
                b.Property(u => u.CreatedAt).IsRequired();
                b.HasMany(u => u.Transactions).WithOne(t => t.User).HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(u => u.Portfolios).WithOne(p => p.User).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(u => u.Wallet).WithOne(w => w.User).HasForeignKey<Wallet>(w => w.UserId);
            });

            // ── Transaction ─────────────────────────────────────────────────
            modelBuilder.Entity<Transaction>(b =>
            {
                b.Property(t => t.Symbol).IsRequired();
                b.Property(t => t.CreatedAt).IsRequired();
            });

            // ── Portfolio ────────────────────────────────────────────────────
            modelBuilder.Entity<Portfolio>(b =>
            {
                b.HasIndex(p => new { p.UserId, p.Symbol }).IsUnique();
                b.Property(p => p.Symbol).IsRequired();
                b.Property(p => p.LastUpdated).IsRequired();
            });

            // ── Wallet ───────────────────────────────────────────────────────
            // FIX: PostgreSQL uses "numeric" not "decimal"
            modelBuilder.Entity<Wallet>(b =>
            {
                b.HasKey(w => w.Id);
                b.Property(w => w.Balance).HasColumnType("numeric(18,4)");
                b.Property(w => w.CreatedAt).IsRequired();
                b.HasIndex(w => w.UserId).IsUnique();
            });

            // ── Seed IDs ─────────────────────────────────────────────────────
            var user1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var user2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

            // ── User Seed ────────────────────────────────────────────────────
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = user1Id,
                    Email = "alice@example.com",
                    FullName = "Alice Doe",
                    PasswordHash = "$2a$12$0/Sr411KPa5E9ZmHWuovPOQM2hpRlLRWw1C8Hv2ps3x4wzWUYEo3q",
                    Provider = AuthProvider.Local,
                    ProviderUserId = null,
                    RefreshTokenHash = null,
                    RefreshTokenExpiryTime = null,
                    EmailOtpHash = null,
                    EmailOtpExpiry = null,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), // FIX: DateTimeKind.Utc
                    LastLoginAt = null,
                    IsActive = true
                },
                new User
                {
                    Id = user2Id,
                    Email = "bob@gmail.com",
                    FullName = "Bob Google",
                    PasswordHash = "$2a$12$0/Sr411KPa5E9ZmHWuovPOQM2hpRlLRWw1C8Hv2ps3x4wzWUYEo3q",
                    Provider = AuthProvider.Google,
                    ProviderUserId = "google-123",
                    RefreshTokenHash = null,
                    RefreshTokenExpiryTime = null,
                    EmailOtpHash = null,
                    EmailOtpExpiry = null,
                    CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc), // FIX: DateTimeKind.Utc
                    LastLoginAt = null,
                    IsActive = true
                }
            );

            // ── Portfolio Seed ───────────────────────────────────────────────
            var portfolio1Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var portfolio2Id = Guid.Parse("44444444-4444-4444-4444-444444444444");

            modelBuilder.Entity<Portfolio>().HasData(
                new Portfolio
                {
                    Id = portfolio1Id,
                    UserId = user1Id,
                    Symbol = "BTC",
                    TotalQuantity = 2.0m,
                    AverageBuyPrice = 25000.00m,
                    LastUpdated = new DateTime(2024, 1, 10, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                },
                new Portfolio
                {
                    Id = portfolio2Id,
                    UserId = user1Id,
                    Symbol = "AAPL",
                    TotalQuantity = 10.0m,
                    AverageBuyPrice = 140.00m,
                    LastUpdated = new DateTime(2024, 1, 11, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                }
            );

            // ── Transaction Seed ─────────────────────────────────────────────
            var tx1 = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var tx2 = Guid.Parse("66666666-6666-6666-6666-666666666666");

            modelBuilder.Entity<Transaction>().HasData(
                new Transaction
                {
                    Id = tx1,
                    UserId = user1Id,
                    Symbol = "BTC",  // FIX: was "bcc" (typo)
                    Type = TransactionType.Buy,
                    Quantity = 1.5m,
                    PriceAtTransaction = 20000m,
                    TotalAmount = 30000m,
                    CreatedAt = new DateTime(2024, 1, 5, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                },
                new Transaction
                {
                    Id = tx2,
                    UserId = user1Id,
                    Symbol = "ETH",  // FIX: was "assdfffds" (garbage value)
                    Type = TransactionType.Buy,
                    Quantity = 10m,
                    PriceAtTransaction = 140m,
                    TotalAmount = 1400m,
                    CreatedAt = new DateTime(2024, 1, 6, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                }
            );

            // ── Wallet Seed ──────────────────────────────────────────────────
            var wallet1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
            var wallet2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");

            modelBuilder.Entity<Wallet>().HasData(
                new Wallet
                {
                    Id = wallet1Id,
                    UserId = user1Id,
                    Balance = 100000.0000m,
                    CreatedAt = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                },
                new Wallet
                {
                    Id = wallet2Id,
                    UserId = user2Id,
                    Balance = 50000.0000m,
                    CreatedAt = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                }
            );

            // ── WalletTransaction Seed ───────────────────────────────────────
            var wtx1 = Guid.Parse("99999999-9999-9999-9999-999999999999");
            var wtx2 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

            modelBuilder.Entity<WalletTransaction>().HasData(
                new WalletTransaction
                {
                    Id = wtx1,
                    WalletId = wallet1Id,
                    Amount = 100000.0000m,
                    Type = WalletTransactionType.Deposit,
                    Purpose = "Initial deposit",
                    TransferId = Guid.Empty,
                    Date = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                },
                new WalletTransaction
                {
                    Id = wtx2,
                    WalletId = wallet2Id,
                    Amount = 50000.0000m,
                    Type = WalletTransactionType.Deposit,
                    Purpose = "Initial deposit",
                    TransferId = Guid.Empty,
                    Date = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                }
            );

            // ── Withdrawal Seed ──────────────────────────────────────────────
            var wd1 = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

            modelBuilder.Entity<Withdrawal>().HasData(
                new Withdrawal
                {
                    Id = wd1,
                    UserId = user1Id,
                    Amount = 2500.00m,
                    Status = WithdrawalStatus.Pending,
                    LocalDateTime = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc) // FIX: DateTimeKind.Utc
                }
            );

            // ── Asset Seed ───────────────────────────────────────────────────
            var asset1 = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

            modelBuilder.Entity<Asset>().HasData(
                new Asset
                {
                    Id = asset1,
                    UserId = user1Id,
                    CoinId = "bitcoin",
                    Quantity = 0.5,
                    BuyPrice = 25000.0
                }
            );

            // ── WatchList Seed ───────────────────────────────────────────────
            // FIX: removed User = null and Coins = new List<Coin>()
            // Navigation properties are NOT allowed inside HasData
            var wl1 = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");

            modelBuilder.Entity<WatchList>().HasData(
                new WatchList
                {
                    Id = wl1,
                    UserId = user1Id
                }
            );

            // ── PaymentOrder Seed ────────────────────────────────────────────
            var pay1 = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

            modelBuilder.Entity<PaymentOrder>().HasData(
                new PaymentOrder
                {
                    Id = pay1,
                    UserId = user1Id,
                    Amount = 49.99m,
                    PaymentMethod = PaymentMethod.Stripe,
                    Status = PaymentOrderStatus.Pending
                }
            );
        }
    }
}