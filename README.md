# Card Merchant System

Bankacılık sektörü için **production-ready** Kart ve Üye İşyeri Yönetim Sistemi.

---

## 🎯 Proje Özeti

**Tamamlanma:** %89 (17/19 modül)  
**Mimari:** Clean Architecture + DDD + CQRS  
**Database:** SQL Server + PostgreSQL (Multi-DB)  
**Resilience:** Polly + Rate Limiting + Redis Cache  
**Monitoring:** Serilog + Elasticsearch + Kibana

---

## 📦 Modül Durumu

### ✅ Tamamlanan (17)

| Modül | Açıklama | Özel Özellik |
|-------|----------|--------------|
| **Card** | Kart başvuru, tahsis, limit | Domain Events (publish edilmiyor ⚠️) |
| **Merchant** | Üye işyeri, terminal yönetimi | 🌟 Multi-DB (SQL Server + PostgreSQL)<br>🌟 CQRS (EF Core + Dapper) |
| **Transaction** | İşlem, provizyon, LKS | 🌟 Polly Resilience<br>🌟 Redis Cache<br>🌟 Rate Limiting |
| **Dispute** | İtiraz yönetimi | - |
| **Campaign** | Kampanya, kural motoru | Rule Engine |
| **BKM** | Switch entegrasyonu | External API Integration |
| **HSM** | Güvenlik modülü (Thales, Gemalto) | Encryption/Decryption |
| **Fee** | Ücret, aidat, tarife yönetimi | Complex calculations |
| **Statement** | Ekstre yönetimi | Document generation |
| **Accounting** | Muhasebe entegrasyonu | Double-entry bookkeeping |
| **MerchantReport** | Üye işyeri raporlama (FTP, mail) | Scheduled reports |
| **MerchantSettlement** | Üye işyeri takas | Payment processing |
| **BulkCardPrint** | Toplu kart basım (Bileşim, Austria) | Batch processing |
| **RegulatoryReporting** | Yasal raporlama (BDDK, TCMB) | Compliance |
| **Courier** | Kurye entegrasyonu (Kuryenet) | Shipment tracking |
| **EarlyBlockResolution** | Erken bloke çözüm | - |
| **WorkOrder** | İş emri yönetimi (Beko, Ingenico, Teknoser) | - |

### ⏳ Planlanmış (2)

| Modül | Öncelik |
|-------|---------|
| **InstantCardPrint** (Evolis Primacy) | Düşük |
| **Inventory** (Envanter yönetimi) | Düşük |

---

## 🏗️ Mimari

```
┌─────────────────────────────────────────────────────────────┐
│                     API Layer (Controllers)                 │
│         JWT Auth, Middleware, Exception Handling            │
├─────────────────────────────────────────────────────────────┤
│                   Application Layer                         │
│    Commands/Queries (MediatR), DTOs, Validators            │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                            │
│   Aggregates, Entities, Value Objects, Smart Enums,        │
│   Domain Events, Business Rules                            │
├─────────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                        │
│  EF Core (Write), Dapper (Read), PostgreSQL, SQL Server    │
│  Redis Cache, Polly Resilience, External APIs              │
└─────────────────────────────────────────────────────────────┘
```

### Kullanılan Pattern'ler

| Pattern | Modül | Açıklama |
|---------|-------|----------|
| **Clean Architecture** | Tümü | Katmanlı mimari, dependency inversion |
| **Domain-Driven Design** | Tümü | Aggregates, Value Objects, Smart Enums |
| **CQRS** | Merchant | EF Core (Write) + Dapper (Read) |
| **Multi-Database** | Merchant | SQL Server + PostgreSQL support |
| **Resilience** | Transaction | Polly (Retry, Circuit Breaker, Timeout) |
| **Repository** | Tümü | Data access abstraction |
| **Mediator** | Tümü | MediatR (commands/queries/events) |
| **Domain Events** | Card, Merchant, Transaction | ⚠️ Tanımlı ama publish edilmiyor |

---

## 🚀 Hızlı Başlangıç

### 1. Gereksinimler

- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 / VS Code
- PgAdmin 4 (opsiyonel)

### 2. Docker Servislerini Başlat

```bash
docker-compose up -d
```

**Çalışan Servisler:**
- PostgreSQL: `localhost:5432`
- PgAdmin: `http://localhost:5050`
- Elasticsearch: `http://localhost:9200`
- Kibana: `http://localhost:5601`

### 3. Database Provider Seç

**appsettings.json:**
```json
{
  "Database": {
    "Provider": "SqlServer",
    "SqlServerConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "PostgreSqlConnection": "Host=localhost;Port=5432;Database=cardmerchantdb;Username=postgres;Password=postgres;"
  }
}
```

### 4. Migration'ları Uygula

**SQL Server (tüm modüller):**
```powershell
# Package Manager Console
Update-Database -Context AuthDbContext -Project CardMerchantSystem.API -StartupProject CardMerchantSystem.API
Update-Database -Context CardDbContext -Project Card.Infrastructure -StartupProject CardMerchantSystem.API
Update-Database -Context MerchantDbContext -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API
Update-Database -Context TransactionDbContext -Project Transaction.Infrastructure -StartupProject CardMerchantSystem.API
# ... (diğer modüller için tekrarla)
```

**PostgreSQL (sadece Merchant modülü):**
```powershell
# appsettings.json'da Provider: "PostgreSql" olmalı
Update-Database -Context MerchantDbContext_Pg -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API
```

### 5. Uygulamayı Çalıştır

```bash
dotnet run --project CardMerchantSystem.API
```

**Erişim:**
- Swagger: `https://localhost:7202/swagger`
- Kibana: `http://localhost:5601`

### 6. Test Kullanıcıları

| Username | Password | Rol |
|----------|----------|-----|
| admin | Admin123! | Admin |
| callcenter | Test123! | CallCenterAgent |

---

## 🛠️ Teknolojiler

| Kategori | Teknoloji |
|----------|-----------|
| **Framework** | .NET 8 |
| **ORM (Write)** | Entity Framework Core 8 |
| **ORM (Read)** | Dapper 2.1 (Merchant modülünde) |
| **Database** | SQL Server + PostgreSQL |
| **Cache** | Redis (Transaction modülü) |
| **Mediator** | MediatR 12 |
| **Resilience** | Polly 8.x (Retry, Circuit Breaker, Timeout) |
| **Rate Limiting** | .NET 8 Built-in |
| **Validation** | FluentValidation |
| **Auth** | JWT + BCrypt |
| **Jobs** | Hangfire |
| **Logging** | Serilog + Elasticsearch + Kibana |
| **Containerization** | Docker Compose |

---

## 🌟 Öne Çıkan Özellikler

### 1. Multi-Database Support (Merchant Modülü)

**Tek kod tabanı, iki veritabanı:**

```
MerchantDbContextBase (Abstract)
    ├── MerchantDbContext (SQL Server)
    └── MerchantDbContext_Pg (PostgreSQL)
```

**Özellikler:**
- Schema-based configuration (dbo vs public)
- Design-time factories
- Automatic type mapping (uniqueidentifier vs uuid)
- Provider-specific conventions

**Provider değiştirme:**
```json
{
  "Database": {
    "Provider": "PostgreSql"  // veya "SqlServer"
  }
}
```

**Detay:** [Merchant.Infrastructure/README.md](Modules/Merchant/Merchant.Infrastructure/README.md)

---

### 2. CQRS (Merchant Modülü)

**Write (EF Core):**
```csharp
// Domain-driven, business logic
var merchant = MerchantAggregate.Create(...);
await _repository.AddAsync(merchant);
await _repository.SaveChangesAsync();
```

**Read (Dapper):**
```csharp
// Performant, optimized queries
var merchants = await _dapperRepository.GetPagedAsync(pageNumber, pageSize);
```

**Test Endpoints:**
```
GET /api/merchant/dapper-test/all
GET /api/merchant/dapper-test/paged?pageNumber=1&pageSize=10
GET /api/merchant/dapper-test/{id}/detail
```

**Avantajlar:**
- ✅ Read performance (Dapper) vs Write integrity (EF Core)
- ✅ Optimized queries için raw SQL
- ✅ Karmaşık JOIN'ler için read model

---

### 3. Resilience Patterns (Transaction Modülü)

**Polly Policies:**
- **Retry:** 3 attempts with exponential backoff
- **Circuit Breaker:** 5 failures → 30s break
- **Timeout:** 30s per request

**Rate Limiting:**
- Fixed window: 100 requests/minute
- Sliding window: 500 requests/5 minutes
- Token bucket: Burst capacity

**Redis Cache:**
- Distributed caching
- Session management
- Rate limit tracking

---

## 📊 Monitoring & Logging

### Elasticsearch + Kibana Stack

**Structured Logging:**
- Correlation ID tracking
- Performance metrics (response time)
- User activity logs
- Database provider usage
- Error tracking

**Kibana Dashboard:** `http://localhost:5601`

**Index Pattern:** `cardmerchant-logs-*`

**Örnek Sorgular (KQL):**
```
# Hata logları
level: "Error"

# Yavaş istekler (>1s)
fields.Elapsed > 1000

# Merchant endpoint'leri
fields.RequestPath: "/api/merchant*"

# PostgreSQL kullanımı
fields.DatabaseProvider: "PostgreSql"

# Belirli kullanıcı
fields.Username: "admin"
```

**Log Seviyeleri:**
- **Debug**: Geliştirme detayları
- **Information**: Normal işlem akışı
- **Warning**: Potansiyel sorunlar
- **Error**: Hatalar ve exception'lar
- **Fatal**: Kritik sistem hataları

---

## 🔐 Güvenlik & Yetkilendirme

### Roller

| Rol | Açıklama |
|-----|----------|
| **Admin** | Tüm yetkiler |
| **CardOperator** | Kart operasyonları |
| **MerchantOperator** | Üye işyeri operasyonları |
| **FinanceOperator** | Finans işlemleri |
| **ComplianceOfficer** | Yasal raporlama |
| **CallCenterAgent** | Çağrı merkezi |
| **Viewer** | Sadece görüntüleme |

### Policy-Based Authorization

```csharp
[Authorize(Policy = Policies.CardManagement)]
public class CardsController : ApiControllerBase
{
    // Sadece CardManagement policy'sine sahip kullanıcılar erişebilir
}
```

**Policy Tanımları:**
- `Policies.CardManagement`: Card modülü operasyonları
- `Policies.MerchantManagement`: Merchant modülü operasyonları
- `Policies.FinanceOperations`: Finans işlemleri
- `Policies.ReportViewing`: Rapor görüntüleme

---

## 📁 Proje Yapısı

```
CardMerchantSystem/
├── docker-compose.yml                    # PostgreSQL + Elasticsearch + Kibana
├── CardMerchantSystem.sln
├── CardMerchantSystem.API/
│   ├── Controllers/                      # API endpoints
│   ├── Middleware/
│   │   ├── GlobalExceptionMiddleware.cs  # Global exception handling
│   │   ├── CorrelationIdMiddleware.cs    # Request tracking
│   │   └── RequestResponseLoggingMiddleware.cs
│   └── Logs/                             # Log files (txt + json)
├── CardMerchantSystem.Shared/
│   ├── Kernel/                           # Base classes
│   │   ├── AggregateRoot.cs
│   │   ├── ValueObject.cs
│   │   ├── SmartEnum.cs
│   │   └── DomainEvent.cs
│   ├── Data/
│   │   ├── DatabaseProvider.cs           # Enum: SqlServer, PostgreSql
│   │   ├── DatabaseOptions.cs
│   │   └── Dapper/                       # Dapper infrastructure
│   ├── Exceptions/                       # Custom exceptions
│   └── Auth/                             # JWT, policies
└── Modules/
    ├── Card/
    │   ├── Card.Domain/
    │   │   ├── Entities/                 # Aggregates, entities
    │   │   ├── Events/                   # Domain events (⚠️ not published)
    │   │   ├── ValueObjects/
    │   │   └── Enums/                    # Smart enums
    │   ├── Card.Application/
    │   │   ├── Commands/                 # Write operations
    │   │   ├── Queries/                  # Read operations
    │   │   └── DTOs/
    │   └── Card.Infrastructure/
    │       ├── Persistence/
    │       │   ├── CardDbContext.cs
    │       │   ├── Configurations/
    │       │   └── Migrations/
    │       └── Repositories/
    ├── Merchant/                         # Multi-DB + CQRS showcase
    │   ├── Merchant.Domain/
    │   │   └── ReadModels/               # Dapper DTOs
    │   └── Merchant.Infrastructure/
    │       ├── Persistence/
    │       │   ├── MerchantDbContextBase.cs
    │       │   ├── MerchantDbContext.cs  # SQL Server
    │       │   ├── MerchantDbContext_Pg.cs # PostgreSQL
    │       │   ├── Migrations/
    │       │   │   ├── SqlServer/
    │       │   │   └── PostgreSql/
    │       │   └── Dapper/
    │       │       └── MerchantDapperContext.cs
    │       └── README.md                 # Multi-DB implementation guide
    ├── Transaction/                      # Resilience showcase
    └── ... (14 more modules)
```

---

## ⚠️ Bilinen Kısıtlamalar

### 1. Domain Events Publish Edilmiyor ⚠️

**Durum:** Event'ler tanımlı ve aggregate'lerde fırlatılıyor, ancak MediatR ile publish edilmiyor!

```csharp
// ✅ Event tanımlı
public class CardApplicationApprovedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public string CustomerTckn { get; }
    // ...
}

// ✅ Aggregate'te fırlatılıyor
public Result Approve(string approverUsername)
{
    Status = CardApplicationStatus.Approved;
    AddDomainEvent(new CardApplicationApprovedEvent(Id, CustomerTckn));
    return Result.Success();
}

// ❌ SaveChangesAsync'te publish EDİLMİYOR
public override async Task<int> SaveChangesAsync(CancellationToken ct)
{
    var result = await base.SaveChangesAsync(ct);
    
    // EKSIK: await _mediator.DispatchDomainEventsAsync(this);
    
    return result;
}
```

**Yapılması Gereken:**
- [ ] `MediatorExtensions.DispatchDomainEventsAsync` metodu ekle
- [ ] DbContext'lere `IMediator` enjekte et
- [ ] `SaveChangesAsync`'te event'leri publish et
- [ ] Event handler'lar yaz (örn: `CardApplicationApprovedEventHandler`)

**Etki:**
- Modüller arası otomatik iletişim yok
- İş akışları manuel API call'lar ile yapılıyor
- Event-driven architecture eksik

---

### 2. Multi-Database Sadece Merchant Modülünde

**Durum:** Sadece Merchant modülü SQL Server + PostgreSQL destekliyor. Diğer 16 modül sadece SQL Server kullanıyor.

**Merchant Modülünü Referans Alarak Diğer Modüllere Ekleme:**

1. Base DbContext oluştur:
```csharp
public abstract class XxxDbContextBase : DbContext
{
    protected abstract void ConfigureModel(ModelBuilder modelBuilder);
    protected abstract string GetSchema();
}
```

2. Provider-specific context'ler:
```csharp
public class XxxDbContext : XxxDbContextBase { }          // SQL Server
public class XxxDbContext_Pg : XxxDbContextBase { }       // PostgreSQL
```

3. Design-time factories ekle
4. Migration'ları oluştur

**Referans:** [Merchant.Infrastructure/README.md](Modules/Merchant/Merchant.Infrastructure/README.md)

---

### 3. CQRS Sadece Merchant Modülünde

**Durum:** Sadece Merchant modülü CQRS pattern kullanıyor (EF Core + Dapper). Diğer modüller sadece EF Core kullanıyor.

**Ne Zaman CQRS Eklenebilir:**
- ✅ Complex join'ler gereken read query'ler
- ✅ Performance kritik read operasyonları
- ✅ Farklı read model ihtiyaçları

---

## 🔧 Geliştirme Notları

### Yeni Migration Oluşturma

**SQL Server:**
```powershell
# appsettings.json: Provider = "SqlServer"
Add-Migration MigrationName_SqlServer -Context XxxDbContext -Project Xxx.Infrastructure -StartupProject CardMerchantSystem.API -OutputDir Persistence\Migrations\SqlServer
Update-Database -Context XxxDbContext -Project Xxx.Infrastructure -StartupProject CardMerchantSystem.API
```

**PostgreSQL (Merchant modülü):**
```powershell
# appsettings.json: Provider = "PostgreSql"
Add-Migration MigrationName_PostgreSql -Context MerchantDbContext_Pg -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API -OutputDir Persistence\Migrations\PostgreSql
Update-Database -Context MerchantDbContext_Pg -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API
```

### Exception Kullanımı

```csharp
// Entity bulunamadı
throw new NotFoundException("Merchant", id);

// İş kuralı ihlali
throw new BusinessRuleException("Merchant aktif değil");

// Conflict (duplicate)
throw new ConflictException("Merchant kodu zaten mevcut");

// Validation hatası
throw new ValidationException("Geçersiz veri");
```

### Logging Best Practices

```csharp
public class ApproveCardHandler : IRequestHandler<ApproveCardCommand, Result>
{
    private readonly ILogger<ApproveCardHandler> _logger;

    public async Task<Result> Handle(ApproveCardCommand request, CancellationToken ct)
    {
        _logger.LogInformation(
            "Kart başvurusu onaylanıyor - ApplicationId: {ApplicationId}, ApprovedBy: {ApprovedBy}",
            request.ApplicationId,
            request.ApprovedBy
        );

        try
        {
            // Business logic
            var result = await _repository.ApproveAsync(request.ApplicationId);
            
            _logger.LogInformation(
                "Kart başvurusu onaylandı - ApplicationId: {ApplicationId}",
                request.ApplicationId
            );
            
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Kart başvurusu onaylanamadı - ApplicationId: {ApplicationId}",
                request.ApplicationId
            );
            throw;
        }
    }
}
```

### Command/Query Pattern

**Command (Write):**
```csharp
public record CreateMerchantCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; }
    public string TaxNumber { get; init; }
    // ...
}

public class CreateMerchantHandler : IRequestHandler<CreateMerchantCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMerchantCommand request, CancellationToken ct)
    {
        // Validation
        // Business logic
        // Save
    }
}
```

**Query (Read):**
```csharp
public record GetMerchantQuery : IRequest<Result<MerchantDto>>
{
    public Guid Id { get; init; }
}

public class GetMerchantHandler : IRequestHandler<GetMerchantQuery, Result<MerchantDto>>
{
    public async Task<Result<MerchantDto>> Handle(GetMerchantQuery request, CancellationToken ct)
    {
        // Read from database
        // Map to DTO
        // Return
    }
}
```

---

## 🐳 Docker

### Servisler

```yaml
services:
  postgres:      # PostgreSQL database
  pgadmin:       # PostgreSQL admin UI
  elasticsearch: # Log storage
  kibana:        # Log visualization
```

### Komutlar

```bash
# Başlat
docker-compose up -d

# Durdur
docker-compose down

# Logları görüntüle
docker-compose logs -f postgres
docker-compose logs -f elasticsearch

# Durum kontrolü
docker-compose ps

# Specific servis restart
docker-compose restart postgres
```

### Database Bağlantısı

**PgAdmin (Docker içinde):**
- URL: `http://localhost:5050`
- Email: `admin@admin.com`
- Password: `admin`
- Host: `postgres` (container name)
- Port: `5432`

**PgAdmin (Lokal):**
- Host: `localhost`
- Port: `5432`
- Database: `cardmerchantdb`
- Username: `postgres`
- Password: `postgres`

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
docker exec -it cardmerchant-postgres psql -U postgres -d cardmerchantdb
```

### Migration Hatası

```powershell
# Migration'ı kaldır
Remove-Migration -Context MerchantDbContext -Project Merchant.Infrastructure

# Database'i temizle
Drop-Database -Context MerchantDbContext -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API

# Yeniden oluştur
Add-Migration InitialCreate_SqlServer -Context MerchantDbContext -Project Merchant.Infrastructure -OutputDir Persistence\Migrations\SqlServer
Update-Database -Context MerchantDbContext -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API
```

### "uniqueidentifier" Hatası (PostgreSQL)

**Sorun:** PostgreSQL'de `uniqueidentifier` tipi yok (SQL Server'a özgü).

**Çözüm:**
1. `appsettings.json`'da `Provider: "PostgreSql"` olduğundan emin ol
2. Migration'ı kaldır: `Remove-Migration`
3. Migration'ı yeniden oluştur (otomatik `uuid` kullanacak)
4. `Update-Database` çalıştır

### Elasticsearch Connection Error

```bash
# Elasticsearch çalışıyor mu?
docker ps | grep elasticsearch

# Elasticsearch logları
docker logs cardmerchant-elasticsearch

# Elasticsearch yeniden başlat
docker-compose restart elasticsearch

# Health check
curl http://localhost:9200/_cluster/health
```

---

## 🚧 Gelecek Geliştirmeler

### Kısa Vadeli (1-3 Ay)

- [ ] **Domain Event Infrastructure**
  - MediatR dispatcher implementation
  - Event handler'lar (CardApplicationApprovedEventHandler, vs.)
  - Cross-module event communication
  
- [ ] **InstantCardPrint Module**
  - Evolis Primacy entegrasyonu
  - Instant card printing workflow
  
- [ ] **Inventory Module**
  - Stock management
  - Card inventory tracking
  - Terminal inventory

- [ ] **Integration Tests**
  - Per-module test suites
  - API integration tests
  - Database integration tests

### Orta Vadeli (3-6 Ay)

- [ ] **Event Sourcing**
  - Dispute modülü için implementation
  - Event store integration
  - Event replay capability

- [ ] **GraphQL API**
  - Query optimization
  - Flexible data fetching
  - Real-time subscriptions

- [ ] **Multi-tenancy**
  - Tenant isolation
  - Tenant-specific databases
  - SaaS support

- [ ] **Real-time Notifications**
  - SignalR integration
  - Push notifications
  - WebSocket support

### Uzun Vadeli (6-12 Ay)

- [ ] **Microservices Migration**
  - Modular monolith → Microservices
  - Service mesh (Istio/Linkerd)
  - API Gateway (Ocelot)

- [ ] **Saga Pattern**
  - Distributed transactions
  - Compensation logic
  - Saga orchestration

- [ ] **gRPC Integration**
  - Inter-service communication
  - Performance optimization
  - Streaming support

- [ ] **API Versioning**
  - Backward compatibility
  - Version management
  - Deprecation strategy

---

## 📖 Ek Dokümantasyon

### Modül-Specific

- **[Merchant Module](Modules/Merchant/Merchant.Infrastructure/README.md)** - Multi-database implementation guide
- **[Transaction Module](Modules/Transaction/Transaction.Infrastructure/README.md)** - Resilience patterns (planlı)
- **[Card Module](Modules/Card/Card.Infrastructure/README.md)** - Domain events (planlı)

### Architecture Decision Records (ADR)

- **[ADR-001: Multi-Database Support](docs/ADR/001-multi-database-support.md)** (planlı)
- **[ADR-002: CQRS Implementation](docs/ADR/002-cqrs-implementation.md)** (planlı)
- **[ADR-003: Resilience Patterns](docs/ADR/003-resilience-patterns.md)** (planlı)
- **[ADR-004: Domain Events](docs/ADR/004-domain-events.md)** (planlı)

---

## 🤝 Katkıda Bulunma

Bu proje eğitim amaçlıdır. Katkıda bulunmak isterseniz:

1. Fork yapın
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit yapın (`git commit -m 'Add amazing feature'`)
4. Push yapın (`git push origin feature/amazing-feature`)
5. Pull Request açın

### Kod Standartları

- Clean Architecture prensiplerine uyun
- SOLID prensiplerini takip edin
- Unit test yazın
- XML documentation ekleyin
- Logging best practices'leri uygulayın

---

## 📄 Lisans

Bu proje eğitim amaçlıdır ve MIT lisansı altında paylaşılmaktadır.

---

## 📞 İletişim

Proje hakkında sorularınız için issue açabilirsiniz.

---

**Son Güncelleme:** 28 Ocak 2026  
**Versiyon:** 1.4.0  
**Tamamlanma:** %89 (17/19 modül)  
**Durum:** ✅ Production-Ready (Domain Events hariç)