# Card Merchant System

Bankacılık sektörü için kapsamlı Kart ve Üye İşyeri Yönetim Sistemi.

---

## 🚀 Hızlı Başlangıç

### Gereksinimler

- .NET 8 SDK
- Docker Desktop (PostgreSQL + Elasticsearch + Kibana için)
- Visual Studio 2022 veya VS Code
- PgAdmin 4 (opsiyonel - PostgreSQL yönetimi için)

### Kurulum

1. **Repository'yi klonla**

2. **Docker servislerini başlat**
```bash
docker-compose up -d
```

**Servisler:**
- PostgreSQL: `localhost:5432`
- PgAdmin: `http://localhost:5050`
- Elasticsearch: `http://localhost:9200`
- Kibana: `http://localhost:5601`

3. **Database provider seç**

`appsettings.json` dosyasını düzenle:
```json
{
  "Database": {
    "Provider": "SqlServer",  // veya "PostgreSql"
    "SqlServerConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSqlConnection": "Host=localhost;Port=5432;Database=cardmerchantdb;Username=postgres;Password=postgres;"
  }
}
```

4. **Migration'ları uygula**

**Package Manager Console** (Visual Studio):
```powershell
# SQL Server için
Add-Migration InitialCreate_SqlServer -Context MerchantDbContext -OutputDir Persistence\Migrations\SqlServer -StartupProject CardMerchantSystem.API
Update-Database -Context MerchantDbContext -StartupProject CardMerchantSystem.API

# PostgreSQL için (appsettings.json'da Provider: "PostgreSql" olmalı)
Add-Migration InitialCreate_PostgreSql -Context MerchantDbContext -OutputDir Persistence\Migrations\PostgreSql -StartupProject CardMerchantSystem.API
Update-Database -Context MerchantDbContext -StartupProject CardMerchantSystem.API

# Diğer modüller için aynı adımları tekrarla...
```

5. **Projeyi çalıştır**
```bash
dotnet run --project CardMerchantSystem.API
```

6. **Uygulamaya eriş**
- Swagger: `https://localhost:7202/swagger`
- Kibana (Logs): `http://localhost:5601`
- PgAdmin: `http://localhost:5050`

### Test Kullanıcıları

| Username | Password | Rol |
|----------|----------|-----|
| admin | Admin123! | Admin |
| callcenter | Test123! | CallCenterAgent |

---

## 📊 Proje Durumu

**Backend Modülleri:** 17/19 tamamlandı (%89)
**Mimari Geliştirmeler:** 4/6 tamamlandı (%67)
**Database Support:** SQL Server + PostgreSQL

> Detaylı durum için: [PROJECT_STATUS.md](PROJECT_STATUS.md)

---

## 🏗️ Mimari
```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│         (Controllers, Middleware, Auth, Logging)            │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                        │
│          (Commands, Queries, DTOs, Handlers)                │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                           │
│       (Entities, Enums, Repositories, Value Objects)        │
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                      │
│  (DbContext, EF Core, Dapper, PostgreSQL, SQL Server)       │
└─────────────────────────────────────────────────────────────┘
```

### Kullanılan Pattern'ler

- **Clean Architecture** - Katmanlı mimari
- **CQRS** - Command Query Responsibility Segregation (EF Core + Dapper)
- **Mediator** - MediatR ile request/handler
- **Repository** - Veri erişim soyutlama
- **Enumeration** - Type-safe enum'lar
- **Multi-Database Support** - SQL Server + PostgreSQL

---

## 📦 Modüller

### Tamamlanan (17)

| Modül | Açıklama | PostgreSQL |
|-------|----------|-----------|
| Card | Kart yönetimi, başvuru, limit | ⏳ |
| Merchant | Üye işyeri, terminal, komisyon | ✅ |
| Transaction | İşlem, provizyon, takas | ⏳ |
| Dispute | İtiraz yönetimi | ⏳ |
| Campaign | Kampanya, kural motoru | ⏳ |
| BKM | Switch entegrasyonu | ⏳ |
| HSM | Güvenlik modülü | ⏳ |
| Fee | Ücret yönetimi | ⏳ |
| Statement | Ekstre yönetimi | ⏳ |
| Accounting | Muhasebe entegrasyonu | ⏳ |
| MerchantReport | Üye işyeri raporlama | ⏳ |
| MerchantSettlement | Üye işyeri takas | ⏳ |
| BulkCardPrint | Toplu kart basım | ⏳ |
| RegulatoryReporting | Yasal raporlama (BDDK/TCMB) | ⏳ |
| Courier | Kurye entegrasyonu | ⏳ |
| EarlyBlockResolution | Erken bloke çözüm | ⏳ |
| WorkOrder | İş emri yönetimi | ⏳ |

### Planlanan (2)

| Modül | Açıklama | Öncelik |
|-------|----------|---------|
| InstantCardPrint | Anında kart basım | Düşük |
| Inventory | Envanter yönetimi | Düşük |

---

## 🗄️ Database Support

### Desteklenen Veritabanları

| Database | Status | Connection String |
|----------|--------|-------------------|
| SQL Server | ✅ | LocalDB veya SQL Server Express |
| PostgreSQL | ✅ | Docker container veya standalone |

### Provider Değiştirme

**appsettings.json:**
```json
{
  "Database": {
    "Provider": "PostgreSql"  // veya "SqlServer"
  }
}
```

Uygulamayı yeniden başlat - otomatik olarak doğru database'i kullanır!

### Migration Stratejisi

Her modül için ayrı migration klasörleri:
```
Merchant.Infrastructure/
└── Persistence/
    └── Migrations/
        ├── SqlServer/
        │   └── 20260124_InitialCreate_SqlServer.cs
        └── PostgreSql/
            └── 20260124_InitialCreate_PostgreSql.cs
```

---

## 🔐 Yetkilendirme

### Roller

| Rol | Açıklama |
|-----|----------|
| Admin | Tüm yetkiler |
| CardOperator | Kart operasyonları |
| MerchantOperator | Üye işyeri operasyonları |
| FinanceOperator | Finans işlemleri |
| ComplianceOfficer | Uyum ve yasal raporlama |
| CallCenterAgent | Çağrı merkezi |
| Viewer | Sadece görüntüleme |

### Policy Kullanımı
```csharp
[Authorize(Policy = Policies.CardManagement)]
public class CardsController : ApiControllerBase
```

---

## 🛠️ Teknolojiler

| Kategori | Teknoloji |
|----------|-----------|
| Framework | .NET 8 |
| ORM (Write) | Entity Framework Core 8 |
| ORM (Read) | Dapper 2.1 |
| Veritabanları | SQL Server + PostgreSQL |
| Cache | Redis (Transaction modülü) |
| Jobs | Hangfire |
| Auth | JWT + BCrypt |
| Validation | FluentValidation |
| Mediator | MediatR 12 |
| Logging | Serilog + Elasticsearch + Kibana |
| Containerization | Docker Compose |

---

## 📊 Loglama ve Monitoring

### Elasticsearch + Kibana Stack

Sistem, Serilog ile structured logging kullanır:

- **Console Logs**: Real-time terminal çıktısı
- **File Logs**: `Logs/` klasöründe txt ve json formatında
- **Elasticsearch**: Merkezi log depolama
- **Kibana**: Log görselleştirme ve analiz

### Log Seviyeleri

- **Debug**: Geliştirme detayları
- **Information**: Normal işlem akışı, HTTP requests
- **Warning**: Potansiyel sorunlar
- **Error**: Hatalar ve exception'lar
- **Fatal**: Kritik sistem hataları

### Kibana Dashboard
```
http://localhost:5601
```

**Özellikler:**
- Request/Response tracking
- Correlation ID ile request takibi
- Performance monitoring (response time)
- Error tracking ve analiz
- User activity logs
- Database provider bilgisi

**Index Pattern:** `cardmerchant-logs-*`

---

## 🚀 CQRS Implementation

### Read Model (Dapper) vs Write Model (EF Core)

**Command (Write) - EF Core:**
```csharp
// Domain-driven, business logic
var merchant = MerchantAggregate.Create(...);
await _repository.AddAsync(merchant);
await _repository.SaveChangesAsync();
```

**Query (Read) - Dapper:**
```csharp
// Performant, optimized queries
var merchants = await _dapperRepository.GetPagedAsync(pageNumber, pageSize);
```

### Merchant Module - Dapper Örneği
```csharp
// Read-only DTO
public class MerchantReadModel { ... }

// Dapper repository
public interface IMerchantDapperRepository
{
    Task<MerchantReadModel?> GetByIdAsync(Guid id);
    Task<IEnumerable<MerchantReadModel>> GetAllAsync();
    Task<(IEnumerable<MerchantReadModel> Data, int TotalCount)> GetPagedAsync(...);
}
```

**Test Endpoints:**
```
GET /api/merchant/dapper-test/all
GET /api/merchant/dapper-test/paged?pageNumber=1&pageSize=10
GET /api/merchant/dapper-test/{id}/detail
```

---

## 📁 Klasör Yapısı
```
CardMerchantSystem/
├── docker-compose.yml                    # PostgreSQL + Elasticsearch + Kibana
├── CardMerchantSystem.sln
├── CardMerchantSystem.API/
│   ├── Logs/                            # Log dosyaları
│   ├── Middleware/
│   │   ├── GlobalExceptionMiddleware.cs
│   │   ├── CorrelationIdMiddleware.cs
│   │   └── RequestResponseLoggingMiddleware.cs
│   └── ...
├── CardMerchantSystem.Shared/
│   ├── Data/
│   │   ├── DatabaseProvider.cs          # Enum: SqlServer, PostgreSql
│   │   ├── DatabaseOptions.cs           # Database configuration
│   │   ├── Extensions/
│   │   │   └── DbContextExtensions.cs   # Multi-database support
│   │   └── Dapper/
│   │       ├── IDapperContext.cs
│   │       ├── BaseDapperRepository.cs
│   │       └── ...
│   └── ...
└── Modules/
    ├── Merchant/
    │   ├── Merchant.Domain/
    │   │   ├── ReadModels/              # Dapper DTOs
    │   │   └── ...
    │   ├── Merchant.Application/
    │   └── Merchant.Infrastructure/
    │       ├── Persistence/
    │       │   ├── MerchantDbContext.cs # Multi-DB support
    │       │   ├── Configurations/       # EF Core configs
    │       │   ├── Migrations/
    │       │   │   ├── SqlServer/
    │       │   │   └── PostgreSql/
    │       │   └── Dapper/
    │       │       └── MerchantDapperContext.cs
    │       └── ...
    └── ... (17 modül)
```

---

## 📝 Geliştirici Notları

### Database Provider Değiştirme

1. `appsettings.json` düzenle:
```json
{
  "Database": {
    "Provider": "PostgreSql"  // veya "SqlServer"
  }
}
```

2. Uygulamayı yeniden başlat
3. Migration gerekiyorsa çalıştır

### Yeni Migration Oluşturma

**SQL Server:**
```powershell
# appsettings.json: Provider = "SqlServer"
Add-Migration MigrationName_SqlServer -Context XxxDbContext -OutputDir Persistence\Migrations\SqlServer
Update-Database -Context XxxDbContext
```

**PostgreSQL:**
```powershell
# appsettings.json: Provider = "PostgreSql"
Add-Migration MigrationName_PostgreSql -Context XxxDbContext -OutputDir Persistence\Migrations\PostgreSql
Update-Database -Context XxxDbContext
```

### Loglama Kullanımı
```csharp
public class MyHandler : IRequestHandler<MyCommand, Result>
{
    private readonly ILogger<MyHandler> _logger;

    public async Task<Result> Handle(MyCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Processing command for {EntityId}", request.Id);
        
        try
        {
            // Business logic
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing command for {EntityId}", request.Id);
            throw;
        }
    }
}
```

### Exception Kullanımı
```csharp
throw new NotFoundException("Entity", id);
throw new BusinessRuleException("Hata mesajı");
throw new ConflictException("Kayıt zaten mevcut");
```

---

## 🐳 Docker Compose

### Servisler
```yaml
# PostgreSQL + Elasticsearch + Kibana
docker-compose up -d
```

**Servis URL'leri:**
- PostgreSQL: `localhost:5432`
- PgAdmin: `http://localhost:5050`
- Elasticsearch: `http://localhost:9200`
- Kibana: `http://localhost:5601`

### Komutlar
```bash
# Başlat
docker-compose up -d

# Durdur
docker-compose down

# Logları görüntüle
docker-compose logs -f

# Durum kontrolü
docker-compose ps

# Specific servis restart
docker-compose restart postgres
```

### PgAdmin Bağlantısı

**Docker içindeki PgAdmin:**
- Host: `postgres` (container name)

**Lokal PgAdmin:**
- Host: `localhost`
- Port: `5432`
- User: `postgres`
- Password: `postgres`

---

## 📊 Monitoring ve Analiz

### Kibana'da Log Analizi

1. **Discover**: http://localhost:5601
2. Index pattern: `cardmerchant-logs-*`
3. **KQL Sorguları:**
```
# Merchant endpoint'leri
fields.RequestPath: "/api/merchant*"

# Error'lar
level: "Error"

# Yavaş requestler
fields.Elapsed > 1000

# Belirli kullanıcı
fields.Username: "admin"

# Database provider
fields.DatabaseProvider: "PostgreSql"
```

### Performance Metrikleri

- Request count
- Response time distribution
- Error rate
- Slow queries (>100ms)
- Database provider usage
- User activity

---

## 🔧 Sorun Giderme

### PostgreSQL Bağlantı Hatası
```bash
# PostgreSQL çalışıyor mu?
docker ps | grep postgres

# PostgreSQL logları
docker logs cardmerchant-postgres

# PostgreSQL yeniden başlat
docker-compose restart postgres

# Connection test
docker exec -it cardmerchant-postgres psql -U postgres
```

### Migration Hatası
```powershell
# Migration'ı kaldır
Remove-Migration -Context MerchantDbContext

# Yeniden oluştur
Add-Migration InitialCreate_PostgreSql -Context MerchantDbContext -OutputDir Persistence\Migrations\PostgreSql

# Database'i güncelle
Update-Database -Context MerchantDbContext
```

### "uniqueidentifier" Hatası

Bu SQL Server type'ı PostgreSQL'de yok. Çözüm:

1. Migration'ı kaldır
2. `appsettings.json`'da provider'ın `PostgreSql` olduğundan emin ol
3. Migration'ı yeniden oluştur (otomatik `uuid` kullanacak)

---

## 📄 Lisans

Bu proje eğitim amaçlıdır.


**Son Güncelleme:** 24 Ocak 2026
**Versiyon:** 1.3.0