using Card.Application;
using Card.Infrastructure;
using Merchant.Application;
using Merchant.Infrastructure;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;";

// Card Module
builder.Services.AddCardApplication();
builder.Services.AddCardInfrastructure(connectionString);

// Merchant Module
builder.Services.AddMerchantApplication();
builder.Services.AddMerchantInfrastructure(connectionString);

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
        Description = "Kart ve Üye Ýþyeri Yönetim Sistemi"
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