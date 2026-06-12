using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stock_Pie.Api.Middleware;
using Stock_Pie.Application.Interfaces;
using Stock_Pie.Application.Mappings;
using Stock_Pie.Application.Services;
using Stock_Pie.Infrastructure.Api;
using Stock_Pie.Infrastructure.Persistence;
using Stock_Pie.Infrastructure.Persistence.Repositories;
using Stock_Pie.Infrastructure.Services;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddMemoryCache();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.UseInlineDefinitionsForEnums();
});
// Add DbContext
//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqloptions =>
//    {
//        sqloptions.CommandTimeout(300); // 5 minutes
//    });
//});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql =>
        {
            npgsql.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null
            );
            npgsql.CommandTimeout(30);
        }
    )
);
// Add HTTP client for Resend
builder.Services.AddHttpClient<IEmailService, SendResendEmailService>(client =>
{
    client.BaseAddress = new Uri("https://api.resend.com/");
});

// CoinGecko HTTP client
//builder.Services.AddHttpClient<ICoinService, CoinService>(c =>
//{
//    c.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
//    c.DefaultRequestHeaders.Add("Accept", "application/json");

//    c.DefaultRequestHeaders.Add(
//        "User-Agent",
//        "StockPieApp/1.0 (ASP.NET Core; contact: sadiklaliwala@email.com)"
//    );

//    var apiKey = builder.Configuration["CoinGecko:ApiKey"] ?? "CG-MmJhR9QuXH2eDQKZk5KXG5np";
//    if (!string.IsNullOrEmpty(apiKey))
//    {
//        c.DefaultRequestHeaders.Add("x-cg-demo-api-key", apiKey);
//    }
//});

builder.Services.AddHttpClient<ICoinService, CoinService>((serviceProvider, c) =>
{
    c.BaseAddress = new Uri("https://api.coingecko.com/api/v3/");
    c.DefaultRequestHeaders.Add("Accept", "application/json");
    c.DefaultRequestHeaders.Add("User-Agent", "StockPieApp/1.0 (ASP.NET Core; contact: sadiklaliwala@email.com)");

    var config = serviceProvider.GetRequiredService<IConfiguration>();
    var apiKey = config["CoinGecko:ApiKey"] ?? "CG-MmJhR9QuXH2eDQKZk5KXG5np";
    if (!string.IsNullOrEmpty(apiKey))
        c.DefaultRequestHeaders.Add("x-cg-demo-api-key", apiKey);
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:5174" };
//Cors Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("vite", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add persistence repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IWithdrawlRepository, WithdrawlRepository>();
builder.Services.AddScoped<IWatchlistRepository, WatchlistRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<ICoinRepository, CoinRepository>();

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPortfolioService, PortfolioService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IWithdrawlService, WithdrawlService>();
builder.Services.AddScoped<IWatchlistService, WatchlistService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add trading service
builder.Services.AddScoped<ITradingService, TradingService>();

// Add application auth services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add email & otp services
//builder.Services.AddScoped<IEmailService, SendResendEmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();

// Add IUserContext and HttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, UserContext>();

builder.Services.AddExceptionHandler<GlobalExceptionMiddleWare>();
builder.Services.AddProblemDetails();

// Add MediatR
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
// Add AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev_secret_change_me";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("vite");

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

// register ApiResponse wrapping middleware after exception handling so exceptions are formatted by exception handler
app.UseMiddleware<Stock_Pie.Api.Middleware.ApiResponseWrappingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
