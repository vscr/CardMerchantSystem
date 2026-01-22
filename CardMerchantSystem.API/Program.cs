using Accounting.Application;
using Accounting.Infrastructure;
using BKM.Application;
using BKM.Infrastructure;
using BulkCardPrint.Application;
using BulkCardPrint.Infrastructure;
using Campaign.Application;
using Campaign.Infrastructure;
using Card.Application;
using Card.Infrastructure;
using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Persistence;
using CardMerchantSystem.API.Auth.Services;
using CardMerchantSystem.API.Jobs;
using CardMerchantSystem.API.Middleware;
using CardMerchantSystem.API.Services;
using Courier.Application;
using Courier.Infrastructure;
using Dispute.Application;
using Dispute.Infrastructure;
using EarlyBlockResolution.Application;
using EarlyBlockResolution.Infrastructure;
using Fee.Application;
using Fee.Infrastructure;
using Hangfire;
using Hangfire.SqlServer;
using HSM.Application;
using HSM.Infrastructure;
using Merchant.Application;
using Merchant.Infrastructure;
using MerchantReport.Application;
using MerchantReport.Infrastructure;
using MerchantSettlement.Application;
using MerchantSettlement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegulatoryReporting.Application;
using RegulatoryReporting.Infrastructure;
using Statement.Application;
using Statement.Infrastructure;
using System.Text;
using Transaction.Application;
using Transaction.Infrastructure;
using WorkOrder.Application;
using WorkOrder.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Connection Strings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;";

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? "localhost:6379";

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization(options =>
{
    // Admin Only
    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole(RoleNames.Admin));

    // Viewer veya üstü (herkes)
    options.AddPolicy(Policies.ViewerOrAbove, policy =>
        policy.RequireRole(
            RoleNames.Admin, RoleNames.CardOperator, RoleNames.MerchantOperator,
            RoleNames.FinanceOperator, RoleNames.ComplianceOfficer,
            RoleNames.CallCenterAgent, RoleNames.Viewer));

    // Kart Yönetimi
    options.AddPolicy(Policies.CardManagement, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.CardOperator));

    // Üye Ýþyeri Yönetimi
    options.AddPolicy(Policies.MerchantManagement, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.MerchantOperator));

    // Finans Yönetimi
    options.AddPolicy(Policies.FinanceManagement, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.FinanceOperator));

    // Uyum Yönetimi
    options.AddPolicy(Policies.ComplianceManagement, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.ComplianceOfficer));

    // Çaðrý Merkezi Eriþimi
    options.AddPolicy(Policies.CallCenterAccess, policy =>
        policy.RequireRole(RoleNames.Admin, RoleNames.CardOperator, RoleNames.CallCenterAgent));

    // Ýþ Emri Yönetimi
    options.AddPolicy(Policies.WorkOrderManagement, policy =>
        policy.RequireRole(
            RoleNames.Admin, RoleNames.CardOperator, RoleNames.MerchantOperator,
            RoleNames.FinanceOperator, RoleNames.CallCenterAgent));
});

// Auth DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Auth Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Menu Service
builder.Services.AddScoped<IMenuService, MenuService>();

// User Service
builder.Services.AddScoped<IUserService, UserService>();

// Dashboard Service
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Card Module
builder.Services.AddCardApplication();
builder.Services.AddCardInfrastructure(connectionString);

// Merchant Module
builder.Services.AddMerchantApplication();
builder.Services.AddMerchantInfrastructure(connectionString);

// Transaction Module
builder.Services.AddTransactionApplication();
builder.Services.AddTransactionInfrastructure(connectionString, redisConnectionString);

// Dispute Module
builder.Services.AddDisputeApplication();
builder.Services.AddDisputeInfrastructure(connectionString);

// Campaign Module
builder.Services.AddCampaignApplication();
builder.Services.AddCampaignInfrastructure(connectionString);

// BKM Module
builder.Services.AddBKMApplication();
builder.Services.AddBKMInfrastructure(connectionString);

// HSM Module
builder.Services.AddHSMApplication();
builder.Services.AddHSMInfrastructure(connectionString);

// Fee Module
builder.Services.AddFeeApplication();
builder.Services.AddFeeInfrastructure(connectionString);

// Statement Module
builder.Services.AddStatementApplication();
builder.Services.AddStatementInfrastructure(connectionString);

// Accounting Module
builder.Services.AddAccountingApplication();
builder.Services.AddAccountingInfrastructure(connectionString);

// MerchantReport Module
builder.Services.AddMerchantReportApplication();
builder.Services.AddMerchantReportInfrastructure(connectionString);

// MerchantReport Module
builder.Services.AddMerchantSettlementApplication();
builder.Services.AddMerchantSettlementInfrastructure(connectionString);

// BulkCardPrint Module
builder.Services.AddBulkCardPrintApplication();
builder.Services.AddBulkCardPrintInfrastructure(connectionString);

// RegulatoryReporting Module
builder.Services.AddRegulatoryReportingApplication();
builder.Services.AddRegulatoryReportingInfrastructure(connectionString);

// Courier Module
builder.Services.AddCourierApplication();
builder.Services.AddCourierInfrastructure(connectionString);

// EarlyBlockResolution Module
builder.Services.AddEarlyBlockResolutionApplication();
builder.Services.AddEarlyBlockResolutionInfrastructure(connectionString);

// WorkOrder Module
builder.Services.AddWorkOrderApplication();
builder.Services.AddWorkOrderInfrastructure(connectionString);

// Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true,
        SchemaName = "Hangfire"
    }));

builder.Services.AddHangfireServer();

// Jobs
builder.Services.AddScoped<SettlementJob>();
builder.Services.AddScoped<DailyLimitResetJob>();
builder.Services.AddScoped<MonthlyLimitResetJob>();

// Controllers
builder.Services.AddControllers();

// Swagger with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Card Merchant System API",
        Version = "v1",
        Description = "Kart ve Üye Ýþyeri Yönetim Sistemi - LKS, Fraud, Takas, Ýtiraz, Kampanya, BKM Switch, HSM"
    });

    // JWT için Swagger ayarý
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Token giriniz. Örnek: Bearer {token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Global Exception Handler
app.UseGlobalExceptionHandler();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Card Merchant System API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Card Merchant System - Jobs"
});

// Recurring Jobs
RecurringJob.AddOrUpdate<SettlementJob>(
    "daily-settlement",
    job => job.ExecuteAsync(),
    "55 23 * * *");

RecurringJob.AddOrUpdate<DailyLimitResetJob>(
    "daily-limit-reset",
    job => job.ExecuteAsync(),
    "1 0 * * *");

RecurringJob.AddOrUpdate<MonthlyLimitResetJob>(
    "monthly-limit-reset",
    job => job.ExecuteAsync(),
    "5 0 1 * *");

app.MapControllers();

app.Run();