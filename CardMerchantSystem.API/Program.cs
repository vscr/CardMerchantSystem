using Card.Application;
using Card.Infrastructure;
using Merchant.Application;
using Merchant.Infrastructure;
using Transaction.Application;
using Transaction.Infrastructure;
using CardMerchantSystem.API.Jobs;
using Hangfire;
using Hangfire.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Connection Strings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;";

var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? "localhost:6379";

// Card Module
builder.Services.AddCardApplication();
builder.Services.AddCardInfrastructure(connectionString);

// Merchant Module
builder.Services.AddMerchantApplication();
builder.Services.AddMerchantInfrastructure(connectionString);

// Transaction Module
builder.Services.AddTransactionApplication();
builder.Services.AddTransactionInfrastructure(connectionString, redisConnectionString);

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

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Card Merchant System API",
        Version = "v1",
        Description = "Kart ve Üye Ýþyeri Yönetim Sistemi - LKS, Fraud, Takas"
    });
});

var app = builder.Build();

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
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Card Merchant System - Jobs",
    // Production'da authorization eklenecek
    // Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Recurring Jobs (Cron schedule)
RecurringJob.AddOrUpdate<SettlementJob>(
    "daily-settlement",
    job => job.ExecuteAsync(),
    "55 23 * * *"); // Her gün 23:55'te

RecurringJob.AddOrUpdate<DailyLimitResetJob>(
    "daily-limit-reset",
    job => job.ExecuteAsync(),
    "1 0 * * *"); // Her gün 00:01'de

RecurringJob.AddOrUpdate<MonthlyLimitResetJob>(
    "monthly-limit-reset",
    job => job.ExecuteAsync(),
    "5 0 1 * *"); // Her ayýn 1'inde 00:05'te

app.MapControllers();

app.Run();