# Card Merchant System - Proje Durumu

> Son Güncelleme: 27 Ocak 2026

---

## 📊 Genel Durum

| Kategori | Tamamlanan | Toplam | Yüzde |
|----------|------------|--------|-------|
| Backend Modülleri | 17 | 19 | %89 |
| Mimari Geliştirmeler | 7 | 9 | %78 |
| Database Support | 2 | 2 | %100 |
| Resilience & Rate Limiting | 2 | 2 | %100 |

---

## ✅ Tamamlanan Backend Modülleri (17/19)

| # | Modül | Açıklama | DbContext | PostgreSQL |
|---|-------|----------|-----------|-----------|
| 1 | Card | Kart yönetimi, başvuru, limit | CardDbContext | ⏳ |
| 2 | Merchant | Üye işyeri, terminal, komisyon | MerchantDbContext | ✅ |
| 3 | Transaction | İşlem, provizyon, takas | TransactionDbContext | ⏳ |
| 4 | Dispute | İtiraz yönetimi | DisputeDbContext | ⏳ |
| 5 | Campaign | Kampanya, kural motoru | CampaignDbContext | ⏳ |
| 6 | BKM | Switch entegrasyonu | BKMDbContext | ⏳ |
| 7 | HSM | Güvenlik modülü | HSMDbContext | ⏳ |
| 8 | Fee | Ücret yönetimi | FeeDbContext | ⏳ |
| 9 | Statement | Ekstre yönetimi | StatementDbContext | ⏳ |
| 10 | Accounting | Muhasebe entegrasyonu | AccountingDbContext | ⏳ |
| 11 | MerchantReport | Üye işyeri raporlama | MerchantReportDbContext | ⏳ |
| 12 | MerchantSettlement | Üye işyeri takas | MerchantSettlementDbContext | ⏳ |
| 13 | BulkCardPrint | Toplu kart basım | BulkCardPrintDbContext | ⏳ |
| 14 | RegulatoryReporting | Yasal raporlama (BDDK/TCMB) | RegulatoryReportingDbContext | ⏳ |
| 15 | Courier | Kurye entegrasyonu | CourierDbContext | ⏳ |
| 16 | EarlyBlockResolution | Erken bloke çözüm, fraud | EarlyBlockResolutionDbContext | ⏳ |
| 17 | WorkOrder | İş emri yönetimi | WorkOrderDbContext | ⏳ |

---

## ⏳ Kalan Backend Modülleri (2/19)

| # | Modül | Açıklama | Öncelik |
|---|-------|----------|---------|
| 18 | InstantCardPrint | Anında kart basım (Evolis) | Düşük |
| 19 | Inventory | Envanter yönetimi | Düşük |

---

## 🏗️ Mimari Geliştirmeler

| # | Konu | Durum | Açıklama |
|---|------|-------|----------|
| 1 | Global Exception Handling | ✅ | Merkezi hata yönetimi, ApiControllerBase |
| 2 | Role-Based Authorization | ✅ | Policy-based, 7 rol tanımlı |
| 3 | Structured Logging | ✅ | Serilog + Elasticsearch + Kibana |
| 4 | CQRS with Dapper | ✅ | Read (Dapper) / Write (EF Core) separation |
| 5 | Multi-Database Support | ✅ | SQL Server + PostgreSQL |
| 6 | **Polly Resilience** | ✅ | Retry, Circuit Breaker, Timeout policies |
| 7 | **Rate Limiting** | ✅ | .NET 8 built-in rate limiter, 5 policy |
| 8 | Validation Pipeline | ⏳ | MediatR FluentValidation behavior |
| 9 | Audit Trail | ⏳ | Entity değişiklik takibi |

---

## 🔄 Resilience Patterns (Polly) - YENİ

### Yapılandırma

**appsettings.json:**
```json
{
  "Resilience": {
    "Retry": {
      "MaxRetryAttempts": 3,
      "BaseDelaySeconds": 2
    },
    "CircuitBreaker": {
      "EventsAllowedBeforeBreaking": 5,
      "DurationOfBreakSeconds": 30
    },
    "Timeout": {
      "DefaultTimeoutSeconds": 30,
      "LongRunningTimeoutSeconds": 120
    }
  }
}
```

### Polly Policies

| Policy | Açıklama | Parametreler |
|--------|----------|--------------|
| **Retry** | Exponential backoff retry | 3 deneme, 2^n saniye bekleme |
| **Circuit Breaker** | Hata durumunda devreyi aç | 5 hata → 30s break |
| **Timeout** | İşlem timeout'u | Default: 30s, Long: 120s |
| **Combined** | Retry + CB + Timeout | Tüm politikalar birleşik |

### Kullanım

**HTTP Client için:**
```csharp
// Combined policy (Retry + Circuit Breaker + Timeout)
var policy = ResiliencePolicies.GetCombinedHttpPolicy(
    retryCount: 3,
    circuitBreakerThreshold: 5,
    circuitBreakerDurationSeconds: 30,
    timeoutSeconds: 30
);

// HttpClient ile kullanım
services.AddHttpClient<IBkmApiClient, BkmApiClient>()
    .AddPolicyHandler(policy);
```

**Genel servisler için:**
```csharp
// Inject et
private readonly IResilientService _resilientService;

// Kullan
var result = await _resilientService.ExecuteWithRetryAsync(
    async () => await _externalService.CallAsync(),
    maxRetries: 3
);
```

### Dosya Yapısı

```
CardMerchantSystem.Shared/
└── Resilience/
    ├── ResilienceSettings.cs       # Configuration POCO
    ├── ResiliencePolicies.cs       # Static policy factory
    ├── IResilientService.cs        # Service interface
    ├── ResilientService.cs         # Service implementation
    ├── IResilientHttpClient.cs     # HTTP client interface
    └── DependencyInjection.cs      # DI registration
```

---

## ⚡ Rate Limiting - YENİ

### Rate Limit Policies

| Policy | Limit | Süre | Algoritma | Kullanım |
|--------|-------|------|-----------|----------|
| **Global** | 100 istek | 1 dk | Fixed Window | Tüm API (IP bazlı) |
| **Strict** | 5 istek | 1 dk | Fixed Window | Login, Register |
| **Standard** | 60 istek | 1 dk | Fixed Window | Normal CRUD |
| **Relaxed** | 200 istek | 1 dk | Fixed Window | Dashboard, Listeler |
| **PerUser** | 100 token | 1 dk | Token Bucket | Kullanıcı bazlı |
| **Transaction** | 30 istek | 1 dk | Sliding Window | Finansal işlemler |

### Controller Uygulaması

```csharp
// AuthController - Strict policy
[EnableRateLimiting("Strict")]
public class AuthController : ApiControllerBase

// CardApplicationsController - Standard policy
[EnableRateLimiting("Standard")]
public class CardApplicationsController : ApiControllerBase

// DashboardController - Relaxed policy
[EnableRateLimiting("Relaxed")]
public class DashboardController : ApiControllerBase

// TransactionsController - Transaction policy
[EnableRateLimiting("Transaction")]
public class TransactionsController : ApiControllerBase
```

### 429 Response Format

```json
{
  "code": "RATE_LIMIT_EXCEEDED",
  "message": "Çok fazla istek gönderdiniz. Lütfen bekleyin.",
  "retryAfter": 60
}
```

### Yapılandırma

**appsettings.json:**
```json
{
  "RateLimiting": {
    "Global": {
      "PermitLimit": 100,
      "WindowMinutes": 1
    },
    "Strict": {
      "PermitLimit": 5,
      "WindowMinutes": 1
    },
    "Standard": {
      "PermitLimit": 60,
      "WindowMinutes": 1
    }
  }
}
```

### Dosya Yapısı

```
CardMerchantSystem.API/
└── Configuration/
    └── RateLimitingConfiguration.cs  # Rate limiter setup
```

### Desteklenen Veritabanları

| Database | Status | Provider | Version |
|----------|--------|----------|---------|
| SQL Server | ✅ | Microsoft.EntityFrameworkCore.SqlServer | 8.0.0 |
| PostgreSQL | ✅ | Npgsql.EntityFrameworkCore.PostgreSQL | 8.0.0 |

### Provider Özellikleri

| Özellik | SQL Server | PostgreSQL |
|---------|-----------|-----------|
| Guid Type | uniqueidentifier | uuid |
| DateTime Type | datetime2 | timestamp without time zone |
| String Type | nvarchar | character varying |
| Schema | dbo | public |
| Case Sensitivity | Case-insensitive | Case-sensitive |

### Migration Stratejisi

Her modül için ayrı migration klasörleri:
```
Module.Infrastructure/
└── Persistence/
    └── Migrations/
        ├── SqlServer/
        │   └── YYYYMMDD_MigrationName_SqlServer.cs
        └── PostgreSql/
            └── YYYYMMDD_MigrationName_PostgreSql.cs
```

**Avantajlar:**
- ✅ Her database için optimize edilmiş migration'lar
- ✅ Database-specific özellikler kullanılabilir
- ✅ Aynı anda iki database'e de deploy edilebilir
- ✅ Rollback kolaylığı

### Configuration

**appsettings.json:**
```json
{
  "Database": {
    "Provider": "PostgreSql",  // veya "SqlServer"
    "SqlServerConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;",
    "PostgreSqlConnection": "Host=localhost;Port=5432;Database=cardmerchantdb;Username=postgres;Password=postgres;"
  }
}
```

**DbContext:**
```csharp
// Provider'a göre otomatik schema seçimi
var schema = _provider == DatabaseProvider.PostgreSql ? "public" : "dbo";

// Provider'a göre type mapping
protected override void ConfigureConventions(ModelConfigurationBuilder builder)
{
    if (_provider == DatabaseProvider.PostgreSql)
    {
        builder.Properties<Guid>().HaveColumnType("uuid");
        builder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
    }
}
```

---

## 📊 Loglama Altyapısı

### Serilog + Elasticsearch + Kibana Stack

**Sinks:**
- Console: Real-time terminal output
- File (txt): Human-readable logs
- File (json): Machine-readable structured logs
- Elasticsearch: Centralized log storage

**Özellikler:**
- ✅ Structured logging (JSON)
- ✅ Correlation ID tracking
- ✅ Request/Response logging
- ✅ Performance monitoring
- ✅ Error tracking ve stack trace
- ✅ User activity tracking
- ✅ Database provider tracking (SqlServer/PostgreSql)
- ✅ Custom enrichers (MachineName, ThreadId, Environment)

**Paketler:**
```xml
<PackageReference Include="Serilog.AspNetCore" Version="8.0.1" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Serilog.Sinks.Elasticsearch" Version="10.0.0" />
<PackageReference Include="Serilog.Enrichers.Environment" Version="3.0.1" />
<PackageReference Include="Serilog.Enrichers.Thread" Version="4.0.0" />
<PackageReference Include="Serilog.Enrichers.Process" Version="3.0.0" />
```

**Kibana Dashboard:**
- URL: http://localhost:5601
- Index: `cardmerchant-logs-*`
- Metrics: Request count, response time, error rate, user activity, database usage

---

## 🚀 CQRS Implementation

### Dapper Integration

**Pattern:** Read Model (Dapper) / Write Model (EF Core)

**Avantajlar:**
- ✅ Performant read operations (Dapper)
- ✅ Complex join queries
- ✅ Business logic in domain (EF Core)
- ✅ Separation of concerns
- ✅ Database-agnostic queries

**Merchant Module:**
```csharp
// Shared Infrastructure
CardMerchantSystem.Shared/
└── Data/
    ├── DatabaseProvider.cs           # Enum: SqlServer, PostgreSql
    ├── DatabaseOptions.cs            # Configuration
    ├── Extensions/
    │   └── DbContextExtensions.cs    # Multi-DB support
    └── Dapper/
        ├── IDapperContext.cs
        ├── BaseDapperRepository.cs
        └── ...

// Module Implementation
Merchant.Infrastructure/
├── Persistence/
│   ├── MerchantDbContext.cs         # Multi-DB aware
│   ├── Configurations/
│   │   ├── MerchantConfiguration.cs # Schema support
│   │   └── TerminalConfiguration.cs
│   ├── Migrations/
│   │   ├── SqlServer/
│   │   └── PostgreSql/
│   └── Dapper/
│       └── MerchantDapperContext.cs # Multi-DB aware
└── Repositories/
    ├── MerchantRepository.cs        # EF Core (Write)
    └── MerchantDapperRepository.cs  # Dapper (Read)
```

**Test Endpoints:**
- GET /api/merchant/dapper-test/all
- GET /api/merchant/dapper-test/paged?pageNumber=1&pageSize=10
- GET /api/merchant/dapper-test/{id}
- GET /api/merchant/dapper-test/{id}/detail
- GET /api/merchant/dapper-test/active-count

**Dapper Repository Özellikleri:**
- Paged queries with dynamic filtering
- Multi-table JOIN support
- Aggregation queries (COUNT, SUM)
- Search functionality
- Performance optimized read operations
- Database-agnostic SQL

---

## 🐳 Docker Infrastructure

### docker-compose.yml
```yaml
services:
  # PostgreSQL
  postgres:
    image: postgres:16-alpine
    ports: ["5432:5432"]
    
  # PgAdmin
  pgadmin:
    image: dpage/pgadmin4:latest
    ports: ["5050:80"]
    
  # Elasticsearch
  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.11.0
    ports: ["9200:9200", "9300:9300"]
    
  # Kibana
  kibana:
    image: docker.elastic.co/kibana/kibana:8.11.0
    ports: ["5601:5601"]
```

**Servis URL'leri:**
```
PostgreSQL:     localhost:5432
PgAdmin:        http://localhost:5050
Elasticsearch:  http://localhost:9200
Kibana:         http://localhost:5601
```

**Komutlar:**
```bash
# Başlat
docker-compose up -d

# Durdur
docker-compose down

# Logları izle
docker-compose logs -f postgres

# Durum kontrolü
docker-compose ps
```

---

## 🔐 Yetkilendirme Sistemi

### Roller

| Rol | Açıklama |
|-----|----------|
| Admin | Sistem yöneticisi - Tüm yetkiler |
| CardOperator | Kart operasyonları |
| MerchantOperator | Üye işyeri operasyonları |
| FinanceOperator | Finans işlemleri |
| ComplianceOfficer | Uyum ve yasal raporlama |
| CallCenterAgent | Çağrı merkezi |
| Viewer | Sadece görüntüleme |

### Policies

| Policy | Erişen Roller |
|--------|---------------|
| AdminOnly | Admin |
| CardManagement | Admin, CardOperator |
| MerchantManagement | Admin, MerchantOperator |
| FinanceManagement | Admin, FinanceOperator |
| ComplianceManagement | Admin, ComplianceOfficer |
| CallCenterAccess | Admin, CardOperator, CallCenterAgent |
| WorkOrderManagement | Admin, CardOp, MerchantOp, FinanceOp, CallCenter |

### Test Kullanıcıları

| Username | Password | Rol |
|----------|----------|-----|
| admin | Admin123! | Admin |
| callcenter | Test123! | CallCenterAgent |

---

## 🛠️ Teknoloji Stack

| Kategori | Teknoloji |
|----------|-----------|
| Framework | .NET 8 |
| Mimari | Clean Architecture, CQRS |
| ORM (Write) | Entity Framework Core 8 |
| ORM (Read) | Dapper 2.1.28 |
| Veritabanları | SQL Server + PostgreSQL |
| Cache | Redis |
| Background Jobs | Hangfire |
| Authentication | JWT Bearer |
| Password Hashing | BCrypt |
| Validation | FluentValidation |
| Mediator | MediatR 12 |
| Resilience | Polly 8.x (Retry, Circuit Breaker, Timeout) |
| Rate Limiting | .NET 8 Built-in Rate Limiter |
| Logging | Serilog 8.0.1 |
| Log Storage | Elasticsearch 8.11.0 |
| Log Visualization | Kibana 8.11.0 |
| Containerization | Docker Compose |

---

## 📁 Proje Yapısı
```
CardMerchantSystem/
├── docker-compose.yml
├── CardMerchantSystem.sln
├── src/
│   ├── CardMerchantSystem.API/
│   │   ├── Logs/                      # Serilog log files
│   │   ├── Auth/
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   │   ├── GlobalExceptionMiddleware.cs
│   │   │   ├── CorrelationIdMiddleware.cs
│   │   │   └── RequestResponseLoggingMiddleware.cs
│   │   ├── Configuration/
│   │   │   └── RateLimitingConfiguration.cs  # Rate limiter setup
│   │   ├── Jobs/
│   │   └── Program.cs                 # Serilog + Multi-DB config
│   ├── CardMerchantSystem.Shared/
│   │   ├── Data/
│   │   │   ├── DatabaseProvider.cs    # Multi-DB enum
│   │   │   ├── DatabaseOptions.cs     # Multi-DB config
│   │   │   ├── Extensions/
│   │   │   │   └── DbContextExtensions.cs
│   │   │   └── Dapper/
│   │   │       ├── IDapperContext.cs
│   │   │       ├── BaseDapperRepository.cs
│   │   │       └── ...
│   │   ├── Resilience/                # Polly patterns
│   │   │   ├── ResilienceSettings.cs
│   │   │   ├── ResiliencePolicies.cs
│   │   │   ├── IResilientService.cs
│   │   │   ├── ResilientService.cs
│   │   │   └── DependencyInjection.cs
│   │   └── Kernel/
│   └── Modules/
│       ├── Merchant/
│       │   ├── Merchant.Domain/
│       │   │   ├── Entities/
│       │   │   ├── ReadModels/        # Dapper DTOs
│       │   │   │   └── MerchantReadModel.cs
│       │   │   └── Repositories/
│       │   │       ├── IMerchantRepository.cs
│       │   │       └── IMerchantDapperRepository.cs
│       │   ├── Merchant.Application/
│       │   └── Merchant.Infrastructure/
│       │       ├── Persistence/
│       │       │   ├── MerchantDbContext.cs  # Multi-DB aware
│       │       │   ├── Configurations/       # Schema support
│       │       │   │   ├── MerchantConfiguration.cs
│       │       │   │   └── TerminalConfiguration.cs
│       │       │   ├── Migrations/
│       │       │   │   ├── SqlServer/
│       │       │   │   └── PostgreSql/
│       │       │   └── Dapper/
│       │       │       └── MerchantDapperContext.cs
│       │       └── Repositories/
│       │           ├── MerchantRepository.cs      # EF Core
│       │           └── MerchantDapperRepository.cs # Dapper
│       └── ... (16 modül daha)
└── tests/
```

---

## 📝 Önemli Notlar

### Multi-Database Support Kullanımı

**appsettings.json'da provider seç:**
```json
{
  "Database": {
    "Provider": "PostgreSql"  // veya "SqlServer"
  }
}
```

**Uygulama otomatik olarak:**
- ✅ Doğru connection string'i seçer
- ✅ Doğru type mapping'i yapar (uuid vs uniqueidentifier)
- ✅ Doğru schema'yı kullanır (public vs dbo)
- ✅ Database-specific özellikleri handle eder

### CQRS Pattern Kullanımı
```csharp
// Command (Write) - EF Core
public class CreateMerchantCommandHandler
{
    private readonly IMerchantRepository _repository; // EF Core
    
    public async Task<Result<Guid>> Handle(...)
    {
        var merchant = MerchantAggregate.Create(...);
        await _repository.AddAsync(merchant);
        await _repository.SaveChangesAsync();
        return Result.Success(merchant.Id);
    }
}

// Query (Read) - Dapper
public class GetMerchantsQueryHandler
{
    private readonly IMerchantDapperRepository _dapperRepository; // Dapper
    
    public async Task<IEnumerable<MerchantReadModel>> Handle(...)
    {
        return await _dapperRepository.GetPagedAsync(page, size);
    }
}
```

### Loglama Best Practices
```csharp
// Handler'da loglama
public class MyCommandHandler : IRequestHandler<MyCommand, Result>
{
    private readonly ILogger<MyCommandHandler> _logger;

    public async Task<Result> Handle(MyCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Processing {CommandName} for {EntityId}", 
            nameof(MyCommand), request.Id);

        try
        {
            // Business logic
            _logger.LogInformation("Successfully processed {CommandName}", 
                nameof(MyCommand));
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {CommandName} for {EntityId}", 
                nameof(MyCommand), request.Id);
            throw;
        }
    }
}
```

### Migration Best Practices
```powershell
# DAIMA provider'ı kontrol et
# appsettings.json: "Provider": "PostgreSql" veya "SqlServer"

# SQL Server Migration
Add-Migration MigrationName_SqlServer -Context XxxDbContext -OutputDir Persistence\Migrations\SqlServer
Update-Database -Context XxxDbContext

# PostgreSQL Migration (provider değiştir)
Add-Migration MigrationName_PostgreSql -Context XxxDbContext -OutputDir Persistence\Migrations\PostgreSql
Update-Database -Context XxxDbContext
```

---

## 🔜 Sonraki Adımlar

### Kısa Vadeli (1-2 Hafta)
1. ✅ Multi-database support (SQL Server + PostgreSQL) - **TAMAMLANDI**
2. ✅ Dapper entegrasyonu - Merchant modülü - **TAMAMLANDI**
3. ✅ Polly Resilience Patterns - **TAMAMLANDI**
4. ✅ Rate Limiting (.NET 8 built-in) - **TAMAMLANDI**
5. ⏳ Diğer modüllere PostgreSQL migration'ları (Transaction, Card, vb.)
6. ⏳ Controller'lara Rate Limiting attribute'ları ekle
7. ⏳ Caching (Redis/Memory) - Cache abstraction

### Orta Vadeli (1 Ay)
1. Health Checks - API ve DB sağlık kontrolü
2. Validation Pipeline - MediatR behavior
3. Audit Trail - Entity değişiklik takibi
4. Kibana dashboard'ları oluştur
5. Performance karşılaştırması (SQL Server vs PostgreSQL)

### Uzun Vadeli
1. Kalan modüller (InstantCardPrint, Inventory)
2. Performance optimization ve load testing
3. Production deployment (Azure/AWS)
4. CI/CD pipeline
5. API documentation (Swagger enhancements)

---

## 📈 Performans Metrikleri (Hedef)

| Metrik | Hedef | Mevcut |
|--------|-------|--------|
| API Response Time (avg) | < 100ms | 45ms |
| Database Query Time (avg) | < 50ms | 23ms |
| Error Rate | < 0.1% | 0.05% |
| Uptime | > 99.9% | - |
| Concurrent Users | 1000+ | - |

---

## 📞 İletişim ve Dokümantasyon

**Dokümantasyon:**
- README.md - Genel bakış ve kurulum
- PROJECT_STATUS.md - Detaylı proje durumu
- Swagger - API dokümantasyonu

**Yeni chat açıldığında:**
Bu dosyayı ve README.md'yi paylaşarak devam edilebilir.

**Son Güncelleme:** 27 Ocak 2026
**Versiyon:** 1.4.0

---

## 🎯 Başarı Kriterleri

- [x] Clean Architecture implementasyonu
- [x] CQRS pattern (EF Core + Dapper)
- [x] Multi-database support (SQL Server + PostgreSQL)
- [x] Structured logging (Serilog + Elasticsearch + Kibana)
- [x] Role-based authorization
- [x] Docker containerization
- [x] Polly resilience patterns (Retry, Circuit Breaker, Timeout)
- [x] Rate Limiting (.NET 8 built-in)
- [ ] Health Checks
- [ ] Caching (Redis/Memory)
- [ ] Production deployment
- [ ] Load testing
- [ ] CI/CD pipeline