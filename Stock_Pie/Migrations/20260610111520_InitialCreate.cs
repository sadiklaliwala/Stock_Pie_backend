using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Stock_Pie.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Times = table.Column<decimal>(type: "numeric", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: true),
                    Percentage = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    ProviderUserId = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenHash = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailOtpHash = table.Column<string>(type: "text", nullable: true),
                    EmailOtpExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "text", nullable: true),
                    CurrentPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    MarketCap = table.Column<long>(type: "bigint", nullable: false),
                    MarketCapRank = table.Column<int>(type: "integer", nullable: false),
                    FullyDilutedValuation = table.Column<long>(type: "bigint", nullable: true),
                    TotalVolume = table.Column<decimal>(type: "numeric", nullable: false),
                    High24h = table.Column<decimal>(type: "numeric", nullable: false),
                    Low24h = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceChange24h = table.Column<decimal>(type: "numeric", nullable: false),
                    PriceChangePercentage24h = table.Column<decimal>(type: "numeric", nullable: false),
                    MarketCapChange24h = table.Column<decimal>(type: "numeric", nullable: true),
                    MarketCapChangePercentage24h = table.Column<decimal>(type: "numeric", nullable: false),
                    CirculatingSupply = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSupply = table.Column<decimal>(type: "numeric", nullable: true),
                    MaxSupply = table.Column<decimal>(type: "numeric", nullable: true),
                    Ath = table.Column<decimal>(type: "numeric", nullable: false),
                    AthChangePercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    AthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Atl = table.Column<decimal>(type: "numeric", nullable: false),
                    AtlChangePercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    AtlDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RoiId = table.Column<int>(type: "integer", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coins_Roi_RoiId",
                        column: x => x.RoiId,
                        principalTable: "Roi",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderType = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Portfolios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    TotalQuantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    AverageBuyPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Portfolios_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PriceAtTransaction = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WatchLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchLists_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Withdrawals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BankAccountNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Withdrawals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Withdrawals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    BuyPrice = table.Column<double>(type: "double precision", nullable: false),
                    CoinId = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Coins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    CoinId = table.Column<string>(type: "text", nullable: true),
                    BuyPrice = table.Column<double>(type: "double precision", nullable: false),
                    SellPrice = table.Column<double>(type: "double precision", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItem_Coins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coins",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: true),
                    TransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoinWatchList",
                columns: table => new
                {
                    CoinsId = table.Column<string>(type: "text", nullable: false),
                    WatchListsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinWatchList", x => new { x.CoinsId, x.WatchListsId });
                    table.ForeignKey(
                        name: "FK_CoinWatchList_Coins_CoinsId",
                        column: x => x.CoinsId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoinWatchList_WatchLists_WatchListsId",
                        column: x => x.WatchListsId,
                        principalTable: "WatchLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Coins",
                columns: new[] { "Id", "Ath", "AthChangePercentage", "AthDate", "Atl", "AtlChangePercentage", "AtlDate", "CirculatingSupply", "CurrentPrice", "FullyDilutedValuation", "High24h", "Image", "LastUpdated", "Low24h", "MarketCap", "MarketCapChange24h", "MarketCapChangePercentage24h", "MarketCapRank", "MaxSupply", "Name", "PriceChange24h", "PriceChangePercentage24h", "RoiId", "Symbol", "TotalSupply", "TotalVolume" },
                values: new object[,]
                {
                    { "binancecoin", 690m, -10.1m, new DateTime(2021, 5, 10, 0, 0, 0, 0, DateTimeKind.Utc), 0.039m, 1589000m, new DateTime(2017, 10, 19, 0, 0, 0, 0, DateTimeKind.Utc), 153000000.0m, 620m, 95000000000L, 630m, "https://coin-images.coingecko.com/coins/images/825/large/bnb-icon2_2x.png", new DateTime(2026, 4, 17, 12, 10, 53, 0, DateTimeKind.Utc), 600m, 95000000000L, 1500000000m, 1.6m, 4, 200000000.0m, "BNB", 12m, 1.97m, null, "bnb", 153000000.0m, 1800000000m },
                    { "bitcoin", 126080m, -40.0339m, new DateTime(2025, 10, 6, 18, 57, 42, 0, DateTimeKind.Utc), 67.81m, 111397.2809m, new DateTime(2013, 7, 6, 0, 0, 0, 0, DateTimeKind.Utc), 20017090.0m, 75605m, 1512981807039L, 76134m, "https://coin-images.coingecko.com/coins/images/1/large/bitcoin.png?1696501400", new DateTime(2026, 4, 17, 12, 10, 53, 0, DateTimeKind.Utc), 73501m, 1512981807039L, 19430849789m, 1.30098m, 1, 21000000.0m, "Bitcoin", 859.52m, 1.14992m, null, "btc", 20017090.0m, 42502155298m },
                    { "ethereum", 4891m, -17.7m, new DateTime(2021, 11, 10, 14, 24, 19, 0, DateTimeKind.Utc), 0.43m, 935000m, new DateTime(2015, 10, 20, 0, 0, 0, 0, DateTimeKind.Utc), 120000000.0m, 4025m, 483000000000L, 4100m, "https://coin-images.coingecko.com/coins/images/279/large/ethereum.png", new DateTime(2026, 4, 17, 12, 10, 53, 0, DateTimeKind.Utc), 3900m, 483000000000L, 9500000000m, 2.01m, 2, null, "Ethereum", 85.5m, 2.17m, null, "eth", 120000000.0m, 21000000000m },
                    { "tether", 1.32m, -24.2m, new DateTime(2018, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), 0.57m, 75.4m, new DateTime(2015, 3, 2, 0, 0, 0, 0, DateTimeKind.Utc), 102000000000.0m, 1.00m, 102000000000L, 1.01m, "https://coin-images.coingecko.com/coins/images/325/large/Tether.png", new DateTime(2026, 4, 17, 12, 10, 53, 0, DateTimeKind.Utc), 0.99m, 102000000000L, 500000000m, 0.5m, 3, null, "Tether", 0.001m, 0.1m, null, "usdt", 102000000000.0m, 60000000000m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "EmailOtpExpiry", "EmailOtpHash", "FullName", "IsActive", "LastLoginAt", "PasswordHash", "Provider", "ProviderUserId", "RefreshTokenExpiryTime", "RefreshTokenHash" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "alice@example.com", null, null, "Alice Doe", true, null, "$2a$12$0/Sr411KPa5E9ZmHWuovPOQM2hpRlLRWw1C8Hv2ps3x4wzWUYEo3q", 0, null, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "bob@gmail.com", null, null, "Bob Google", true, null, "$2a$12$0/Sr411KPa5E9ZmHWuovPOQM2hpRlLRWw1C8Hv2ps3x4wzWUYEo3q", 1, "google-123", null, null }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "BuyPrice", "CoinId", "Quantity", "UserId" },
                values: new object[] { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), 25000.0, "bitcoin", 0.5, new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "PaymentOrders",
                columns: new[] { "Id", "Amount", "PaymentMethod", "Status", "UserId" },
                values: new object[] { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), 49.99m, 0, 0, new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Portfolios",
                columns: new[] { "Id", "AverageBuyPrice", "LastUpdated", "Symbol", "TotalQuantity", "UserId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), 25000.00m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "BTC", 2.0m, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), 140.00m, new DateTime(2024, 1, 11, 0, 0, 0, 0, DateTimeKind.Utc), "AAPL", 10.0m, new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "CreatedAt", "PriceAtTransaction", "Quantity", "Symbol", "TotalAmount", "Type", "UserId" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Utc), 20000m, 1.5m, "BTC", 30000m, 0, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(2024, 1, 6, 0, 0, 0, 0, DateTimeKind.Utc), 140m, 10m, "ETH", 1400m, 0, new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "CreatedAt", "UserId" },
                values: new object[,]
                {
                    { new Guid("77777777-7777-7777-7777-777777777777"), 100000.0000m, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("88888888-8888-8888-8888-888888888888"), 50000.0000m, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222") }
                });

            migrationBuilder.InsertData(
                table: "WatchLists",
                columns: new[] { "Id", "UserId" },
                values: new object[] { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Withdrawals",
                columns: new[] { "Id", "Amount", "BankAccountNumber", "LocalDateTime", "Status", "UserId" },
                values: new object[] { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 2500.00m, null, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "WalletTransactions",
                columns: new[] { "Id", "Amount", "Date", "Purpose", "TransferId", "Type", "WalletId" },
                values: new object[,]
                {
                    { new Guid("99999999-9999-9999-9999-999999999999"), 100000.0000m, new DateTime(2024, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Initial deposit", new Guid("00000000-0000-0000-0000-000000000000"), 0, new Guid("77777777-7777-7777-7777-777777777777") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 50000.0000m, new DateTime(2024, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), "Initial deposit", new Guid("00000000-0000-0000-0000-000000000000"), 0, new Guid("88888888-8888-8888-8888-888888888888") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CoinId",
                table: "Assets",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_UserId",
                table: "Assets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Coins_RoiId",
                table: "Coins",
                column: "RoiId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinWatchList_WatchListsId",
                table: "CoinWatchList",
                column: "WatchListsId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_CoinId",
                table: "OrderItem",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderId",
                table: "OrderItem",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrders_UserId",
                table: "PaymentOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolios_UserId_Symbol",
                table: "Portfolios",
                columns: new[] { "UserId", "Symbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchLists_UserId",
                table: "WatchLists",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Withdrawals_UserId",
                table: "Withdrawals",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "CoinWatchList");

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "Portfolios");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Withdrawals");

            migrationBuilder.DropTable(
                name: "WatchLists");

            migrationBuilder.DropTable(
                name: "Coins");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Roi");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
