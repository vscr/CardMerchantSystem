
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
using CardMerchantSystem.API.Configuration;
using CardMerchantSystem.API.Jobs;
using CardMerchantSystem.API.Middleware;
using CardMerchantSystem.API.Services;
using CardMerchantSystem.Shared.Data;
using CardMerchantSystem.Shared.Data.Dapper.Extensions;
using CardMerchantSystem.Shared.Resilience;
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
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Statement.Application;
using Statement.Infrastructure;
using System.Text;
using Transaction.Application;
using Transaction.Infrastructure;
using WorkOrder.Application;
using WorkOrder.Infrastructure;

// ══════════════════════════════════════════════════════════════
// SERILOG BOOTSTRAP LOGGER - EN BAŞTA
// ══════════════════════════════════════════════════════════════
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting CardMerchantSystem API...");

    var builder = WebApplication.CreateBuilder(args);

    // ══════════════════════════════════════════════════════════════
    // DATABASE CONFIGURATION
    // ══════════════════════════════════════════════════════════════
    var databaseOptions = builder.Configuration
        .GetSection(DatabaseOptions.SectionName)
        .Get<DatabaseOptions>() ?? new DatabaseOptions();

    Log.Information("Using database provider: {Provider}", databaseOptions.Provider);
    Log.Information("Connection string: {ConnectionString}",
        databaseOptions.GetConnectionString().Substring(0, Math.Min(50, databaseOptions.GetConnectionString().Length)) + "...");

    // Database options'ı servislere ekle
    builder.Services.Configure<DatabaseOptions>(
        builder.Configuration.GetSection(DatabaseOptions.SectionName));


    // ══════════════════════════════════════════════════════════════
    // SERILOG CONFIGURATION
    // ══════════════════════════════════════════════════════════════
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("Application", "CardMerchantSystem")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            //.WriteTo.File(
            //    path: "Logs/log-.txt",
            //    rollingInterval: RollingInterval.Day,
            //    retainedFileCountLimit: 30,
            //    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
            //.WriteTo.File(
            //    path: "Logs/log-.json",
            //    rollingInterval: RollingInterval.Day,
            //    retainedFileCountLimit: 30,
            //    formatter: new Serilog.Formatting.Compact.CompactJsonFormatter())
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(
                context.Configuration["Serilog:WriteTo:4:Args:nodeUris"] ?? "http://localhost:9200"))
            {
                IndexFormat = $"cardmerchant-logs-{context.HostingEnvironment.EnvironmentName.ToLower()}-{{0:yyyy.MM}}",
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                TypeName = null
            });
    });

    // ══════════════════════════════════════════════════════════════
    // CONNECTION STRINGS
    // ══════════════════════════════════════════════════════════════
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;";

    var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
        ?? "localhost:6379";

    // ══════════════════════════════════════════════════════════════
    // JWT AUTHENTICATION
    // ══════════════════════════════════════════════════════════════
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

        // Üye İşyeri Yönetimi
        options.AddPolicy(Policies.MerchantManagement, policy =>
            policy.RequireRole(RoleNames.Admin, RoleNames.MerchantOperator));

        // Finans Yönetimi
        options.AddPolicy(Policies.FinanceManagement, policy =>
            policy.RequireRole(RoleNames.Admin, RoleNames.FinanceOperator));

        // Uyum Yönetimi
        options.AddPolicy(Policies.ComplianceManagement, policy =>
            policy.RequireRole(RoleNames.Admin, RoleNames.ComplianceOfficer));

        // Çağrı Merkezi Erişimi
        options.AddPolicy(Policies.CallCenterAccess, policy =>
            policy.RequireRole(RoleNames.Admin, RoleNames.CardOperator, RoleNames.CallCenterAgent));

        // İş Emri Yönetimi
        options.AddPolicy(Policies.WorkOrderManagement, policy =>
            policy.RequireRole(
                RoleNames.Admin, RoleNames.CardOperator, RoleNames.MerchantOperator,
                RoleNames.FinanceOperator, RoleNames.CallCenterAgent));
    });

    // ══════════════════════════════════════════════════════════════
    // HTTP CONTEXT ACCESSOR (Serilog Enrichers için gerekli)
    // ══════════════════════════════════════════════════════════════
    builder.Services.AddHttpContextAccessor();

    // ══════════════════════════════════════════════════════════════
    // AUTH DBCONTEXT
    // ══════════════════════════════════════════════════════════════
    builder.Services.AddDbContext<AuthDbContext>(options =>
        options.UseSqlServer(connectionString));

    // ══════════════════════════════════════════════════════════════
    // CORS
    // ══════════════════════════════════════════════════════════════
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

    // ══════════════════════════════════════════════════════════════
    // AUTH SERVICES
    // ══════════════════════════════════════════════════════════════
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IMenuService, MenuService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IRoleService, RoleService>();
    builder.Services.AddScoped<IDashboardService, DashboardService>();
    builder.Services.AddScoped<ILocalizationService, LocalizationService>();

    // ══════════════════════════════════════════════════════════════
    // DAPPER TYPE HANDLERS
    // ══════════════════════════════════════════════════════════════
    SqlMapperExtensions.RegisterTypeHandlers();
    SqlMapperExtensions.ConfigureCaseInsensitiveMapping();

    // ══════════════════════════════════════════════════════════════
    // MODULE REGISTRATIONS
    // ══════════════════════════════════════════════════════════════
    Log.Information("Registering modules...");

    // Card Module
    builder.Services.AddCardApplication();
    builder.Services.AddCardInfrastructure(connectionString);

    // Merchant Module
    builder.Services.AddMerchantApplication();
    builder.Services.AddMerchantInfrastructure(builder.Configuration);


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

    // MerchantSettlement Module
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

    // Resilience Services
    builder.Services.AddResilienceServices(builder.Configuration);

    // Rate Limiting
    builder.Services.AddRateLimitingServices();

    Log.Information("All modules registered successfully");

    // ══════════════════════════════════════════════════════════════
    // HANGFIRE
    // ══════════════════════════════════════════════════════════════
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

    //// Jobs
    //builder.Services.AddScoped<SettlementJob>();
    //builder.Services.AddScoped<DailyLimitResetJob>();
    //builder.Services.AddScoped<MonthlyLimitResetJob>();

    // ══════════════════════════════════════════════════════════════
    // CONTROLLERS & SWAGGER
    // ══════════════════════════════════════════════════════════════
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Card Merchant System API",
            Version = "v1",
            Description = "Kart ve Üye İşyeri Yönetim Sistemi - LKS, Fraud, Takas, İtiraz, Kampanya, BKM Switch, HSM"
        });

        // JWT için Swagger ayarı
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

    // ══════════════════════════════════════════════════════════════
    // BUILD APP
    // ══════════════════════════════════════════════════════════════
    var app = builder.Build();

    Log.Information("Application built successfully");

    // ══════════════════════════════════════════════════════════════
    // SERILOG REQUEST LOGGING (En önemli middleware)
    // ══════════════════════════════════════════════════════════════
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
            diagnosticContext.Set("UserId", httpContext.User?.FindFirst("sub")?.Value);
            diagnosticContext.Set("Username", httpContext.User?.Identity?.Name);

            if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                diagnosticContext.Set("CorrelationId", correlationId);
            }
        };
    });

    // ══════════════════════════════════════════════════════════════
    // CUSTOM MIDDLEWARES
    // ══════════════════════════════════════════════════════════════
    app.UseMiddleware<CorrelationIdMiddleware>();
    // app.UseMiddleware<RequestResponseLoggingMiddleware>(); // Opsiyonel - çok detaylı loglama

    // ══════════════════════════════════════════════════════════════
    // GLOBAL EXCEPTION HANDLER
    // ══════════════════════════════════════════════════════════════
    app.UseGlobalExceptionHandler();


    // ══════════════════════════════════════════════════════════════
    // USE RATE LIMITER
    // ══════════════════════════════════════════════════════════════
    app.UseRateLimiter();

    // ══════════════════════════════════════════════════════════════
    // PIPELINE
    // ══════════════════════════════════════════════════════════════
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Card Merchant System API v1");
        });

        using var scope = app.Services.CreateScope();
        //await MigrationHelper.MigrateAllDatabasesAsync(scope.ServiceProvider);
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

    // ══════════════════════════════════════════════════════════════
    // RECURRING JOBS
    // ══════════════════════════════════════════════════════════════
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

    Log.Information("Application starting on {Environment}", app.Environment.EnvironmentName);
    Log.Information("Swagger available at: /swagger");
    Log.Information("Hangfire dashboard available at: /hangfire");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Shutting down application...");
    Log.CloseAndFlush();
}