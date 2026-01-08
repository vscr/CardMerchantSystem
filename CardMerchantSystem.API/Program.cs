using Card.Application;
using Card.Infrastructure;
using Merchant.Application;
using Merchant.Infrastructure;
using Transaction.Application;
using Transaction.Infrastructure;

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
app.MapControllers();

app.Run();