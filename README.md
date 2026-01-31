# Card Merchant System

Bankacılık sektörü için **production-ready** Kart ve Üye İşyeri Yönetim Sistemi.

---

## 🎯 Proje Özeti

**Tamamlanma:** %92 (17/19 modül + Domain Events ✅)  
**Mimari:** Clean Architecture + DDD + CQRS + Event-Driven  
**Database:** SQL Server + PostgreSQL (Multi-DB)  
**Resilience:** Polly + Rate Limiting + Redis Cache  
**Monitoring:** Serilog + Elasticsearch + Kibana

---

## 📦 Modül Durumu

### ✅ Tamamlanan (17)

| Modül | Açıklama | Özel Özellik |
|-------|----------|--------------|
| **Card** | Kart başvuru, tahsis, limit | 🌟 Domain Events (local + integration) |
| **Merchant** | Üye işyeri, terminal yönetimi | 🌟 Multi-DB (SQL Server + PostgreSQL)<br>🌟 CQRS (EF Core + Dapper)<br>🌟 Domain Events (local + integration) |
| **Transaction** | İşlem, provizyon, LKS | 🌟 Polly Resilience<br>🌟 Redis Cache<br>🌟 Rate Limiting<br>🌟 Domain Events (local + integration) |
| **Dispute** | İtiraz yönetimi | 🌟 Cross-module: FraudDetected dinler |
| **Campaign** | Kampanya, kural motoru | 🌟 Cross-module: TransactionCompleted dinler |
| **BKM** | Switch entegrasyonu | 🌟 Cross-module: TerminalActivated dinler |
| **HSM** | Güvenlik modülü (Thales, Gemalto) | 🌟 Cross-module: CardApplicationApproved + TerminalActivated dinler |
| **Fee** | Ücret, aidat, tarife yönetimi | 🌟 Cross-module: TransactionCompleted dinler |
| **Statement** | Ekstre yönetimi | Document generation |
| **Accounting** | Muhasebe entegrasyonu | 🌟 Cross-module: TransactionCompleted dinler |
| **MerchantReport** | Üye işyeri raporlama (FTP, mail) | Scheduled reports |
| **MerchantSettlement** | Üye işyeri takas | Payment processing |
| **BulkCardPrint** | Toplu kart basım (Bileşim, Austria) | 🌟 Cross-module: CardApplicationApproved dinler |
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
│    Event Handlers (Same-Module + Cross-Module)             │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                            │
│   Aggregates, Entities, Value Objects, Smart Enums,        │
│   Domain Events, Integration Events, Business Rules        │
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
| **Domain Events** | Card, Merchant, Transaction | ✅ Local events + Cross-module integration events |

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

### 1. Domain Events (Event-Driven Architecture) ✅

**İki katmanlı event sistem:**

```
Aggregate
    ├── Local Domain Events       → Same-module handler'lar (logging, audit)
    └── Integration Events        → Cross-module handler'lar (iş akışı)
```

#### Infrastructure

```csharp
// MediatorExtensions.cs — DbContext'ten event dispatch
public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context)
{
    var domainEntities = context.ChangeTracker.Entries<AggregateRoot>()
        .Where(x => x.Entity.DomainEvents.Any())
        .Select(x => x.Entity).ToList();

    var domainEvents = domainEntities.SelectMany(x => x.DomainEvents).ToList();
    domainEntities.ForEach(entity => entity.ClearDomainEvents());

    foreach (var domainEvent in domainEvents)
        await mediator.Publish(domainEvent);
}

// DbContext — SaveChangesAsync'te otomatik dispatch
public override async Task<int> SaveChangesAsync(CancellationToken ct)
{
    var result = await base.SaveChangesAsync(ct);
    if (_mediator != null)
        await _mediator.DispatchDomainEventsAsync(this);
    return result;
}
```

#### Integration Events (Shared)

| Event | Tetikleyen | Dinleyen Modüller |
|-------|-----------|-------------------|
| `CardApplicationApprovedIntegrationEvent` | Card.Approve() | BulkCardPrint, HSM |
| `CardPrintedIntegrationEvent` | Card.MarkAsPrinted() | - |
| `MerchantApprovedIntegrationEvent` | Merchant.Approve() | - |
| `TerminalActivatedIntegrationEvent` | Merchant.ActivateTerminal() | HSM, BKM |
| `TransactionCompletedIntegrationEvent` | Transaction.Approve() | Campaign, Fee, Accounting |
| `FraudDetectedIntegrationEvent` | Transaction.SetFraudCheckResult() | Dispute |

#### Same-Module Event Handlers

**Card Module:**
- `CardApplicationApprovedEventHandler` — Onay logu
- `CardApplicationRejectedEventHandler` — Red logu
- `CardPrintedEventHandler` — Basım logu
- `CardDeliveryStartedEventHandler` — Teslimat logu
- `CardApplicationCancelledEventHandler` — İptal logu
- `CardDeliveredEventHandler` — Teslim logu

**Merchant Module:**
- `MerchantApprovedEventHandler` — Onay logu
- `MerchantActivatedEventHandler` — Aktivasyon logu
- `MerchantSuspendedEventHandler` — Askıya alma logu
- `TerminalActivatedEventHandler` — Terminal aktivasyon logu

**Transaction Module:**
- `TransactionCreatedEventHandler` — Oluşturma logu
- `TransactionApprovedEventHandler` — Onay logu
- `TransactionDeclinedEventHandler` — Red logu
- `TransactionSettledEventHandler` — Takas logu
- `FraudDetectedEventHandler` — Fraud uyarı logu

#### Cross-Module Event Handlers

**BulkCardPrint Module:**
- `CardApplicationApprovedIntegrationEventHandler` — Kart onaylandığında basım emri oluşturur

**HSM Module:**
- `CardApplicationApprovedIntegrationEventHandler` — Kart onaylandığında CVV/PIN üretir
- `TerminalActivatedIntegrationEventHandler` — Terminal aktive edildiğinde master/working key üretir

**BKM Module:**
- `TerminalActivatedIntegrationEventHandler` — Terminal aktive edildiğinde BKM Switch'e kaydeder

**Campaign Module:**
- `TransactionCompletedIntegrationEventHandler` — İşlem tamamlandığında kampanya puanları hesaplar

**Fee Module:**
- `TransactionCompletedIntegrationEventHandler` — İşlem tamamlandığında komisyon hesaplar

**Accounting Module:**
- `TransactionCompletedIntegrationEventHandler` — İşlem tamamlandığında muhasebe kayıt oluşturur

**Dispute Module:**
- `FraudDetectedIntegrationEventHandler` — Fraud tespit edildiğinde otomatik dispute oluşturur

#### Test Senaryoları (Doğrulandı ✅)

**Senaryo 1: Terminal Aktivasyon**
```
PUT /api/Merchants/{merchantId}/terminals/{terminalId}/activate
```
```
🎉 [Merchant] Terminal aktiv edildi          ← Local event
🔄 [BKM] Terminal BKM Switch'e kaydediliyor ← Integration event
✅ [BKM] Terminal BKM Switch'e kaydedildi
🔐 [HSM] Terminal key'leri üretiliyor       ← Integration event
✅ [HSM] Terminal key'leri üretildi
```

**Senaryo 2: Transaction Onay (Satış)**
```
POST /api/Transactions
{
  "transactionTypeId": 1,    // Sale
  "amount": 150.50,          // Fraud pass, limit OK
  ...
}
```
```
🆕 [Transaction] İşlem oluşturuldu          ← Local event
✅ [Transaction] İşlem onaylandı            ← Local event
🎯 [Campaign] Kampanya puanları hesaplıyor  ← Integration event
✅ [Campaign] Kampanya puanları hesaplandı
💰 [Fee] Komisyon hesaplıyor                ← Integration event
✅ [Fee] Komisyon hesaplandı
📊 [Accounting] Muhasebe kayıt oluşturuluyor ← Integration event
✅ [Accounting] Muhasebe kayıt oluşturuldu
```

**Senaryo 3: Fraud Reject**
```
POST /api/Transactions
{
  "transactionTypeId": 1,
  "amount": 150000,          // Score: 80 → REJECT
  ...
}
```
```
🆕 [Transaction] İşlem oluşturuldu          ← Local event
⚠️  [Transaction] FRAUD TESPİT EDİLDİ       ← Local event
❌ [Transaction] İşlem reddedildi           ← Local event
⚠️  [Dispute] Fraud dispute oluşturuluyor   ← Integration event
✅ [Dispute] Fraud dispute oluşturuldu
```

**Fraud Score Kuralları:**
| Kural | Koşul | Score |
|-------|-------|-------|
| Yüksek tutar | Amount > 50,000 | +30 |
| Gece saati | 00:00 - 05:00 | +20 |
| Çok yüksek tutar | Amount > 100,000 | +40 |
| Yuvarlak tutar | Tam sayı & > 1,000 | +10 |
| **Reject** | **Score ≥ 80** | **REJECT** |
| Review | Score ≥ 50 | Review |
| Pass | Score < 50 | Pass |

---

### 2. Multi-Database Support (Merchant Modülü)

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
- IMediator injection for domain event dispatch

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

### 3. CQRS (Merchant Modülü)

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

### 4. Resilience Patterns (Transaction Modülü)

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
- Domain event flow tracking

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

# Domain event log'ları
fields.Message: "*[Transaction]*"
fields.Message: "*[Campaign]*"
fields.Message: "*[HSM]*"
```

**Log Seviyeleri:**
- **Debug**: Geliştirme detayları
- **Information**: Normal işlem akışı + Domain event log'ları
- **Warning**: Potansiyel sorunlar (Fraud tespit)
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
│   │   ├── Entity.cs                     # DomainEvents list + ClearDomainEvents
│   │   ├── ValueObject.cs
│   │   ├── Enumeration.cs               # Smart Enums
│   │   ├── IDomainEvent.cs              # INotification marker
│   │   └── DomainEvent.cs               # Base class (EventId, OccurredOn)
│   ├── Events/
│   │   └── IntegrationEvents.cs         # Cross-module integration events
│   ├── Extensions/
│   │   └── MediatorExtensions.cs        # DispatchDomainEventsAsync
│   ├── Data/
│   │   ├── DatabaseProvider.cs           # Enum: SqlServer, PostgreSql
│   │   ├── DatabaseOptions.cs
│   │   └── Dapper/                       # Dapper infrastructure
│   ├── Exceptions/                       # Custom exceptions
│   └── Auth/                             # JWT, policies
└── Modules/
    ├── Card/
    │   ├── Card.Domain/
    │   │   ├── Entities/                 # CardApplication aggregate
    │   │   ├── Events/                   # Local domain events ✅
    │   │   ├── ValueObjects/
    │   │   └── Enums/
    │   ├── Card.Application/
    │   │   ├── Commands/
    │   │   ├── Queries/
    │   │   ├── EventHandlers/            # Same-module event handlers ✅
    │   │   └── DTOs/
    │   └── Card.Infrastructure/
    │       ├── Persistence/
    │       │   ├── CardDbContext.cs      # IMediator + DispatchDomainEventsAsync ✅
    │       │   ├── Configurations/
    │       │   └── Migrations/
    │       └── Repositories/
    ├── Merchant/                         # Multi-DB + CQRS + Events showcase
    │   ├── Merchant.Domain/
    │   │   ├── Events/                   # Local domain events ✅
    │   │   └── ReadModels/               # Dapper DTOs
    │   ├── Merchant.Application/
    │   │   └── EventHandlers/            # Same-module event handlers ✅
    │   └── Merchant.Infrastructure/
    │       ├── Persistence/
    │       │   ├── MerchantDbContextBase.cs  # IMediator + dispatch ✅
    │       │   ├── MerchantDbContext.cs      # SQL Server
    │       │   ├── MerchantDbContext_Pg.cs   # PostgreSQL
    │       │   ├── Migrations/
    │       │   │   ├── SqlServer/
    │       │   │   └── PostgreSql/
    │       │   └── Dapper/
    │       └── README.md
    ├── Transaction/                      # Resilience + Events showcase
    │   ├── Transaction.Domain/
    │   │   ├── Entities/                 # TransactionAggregate
    │   │   ├── Events/                   # Local domain events ✅
    │   │   ├── Services/                 # IFraudService, ILimitService
    │   │   └── ValueObjects/
    │   ├── Transaction.Application/
    │   │   ├── Commands/                 # ProcessTransactionCommand (auto-approve flow)
    │   │   └── EventHandlers/            # Same-module event handlers ✅
    │   └── Transaction.Infrastructure/
    │       ├── Persistence/
    │       │   └── TransactionDbContext.cs   # IMediator + dispatch ✅
    │       └── Services/                     # FraudService, LimitService
    ├── BulkCardPrint/
    │   └── BulkCardPrint.Application/
    │       └── EventHandlers/            # Cross-module: CardApplicationApproved ✅
    ├── HSM/
    │   └── HSM.Application/
    │       └── EventHandlers/            # Cross-module: CardApproved + TerminalActivated ✅
    ├── BKM/
    │   └── BKM.Application/
    │       └── EventHandlers/            # Cross-module: TerminalActivated ✅
    ├── Campaign/
    │   └── Campaign.Application/
    │       └── EventHandlers/            # Cross-module: TransactionCompleted ✅
    ├── Fee/
    │   └── Fee.Application/
    │       └── EventHandlers/            # Cross-module: TransactionCompleted ✅
    ├── Accounting/
    │   └── Accounting.Application/
    │       └── EventHandlers/            # Cross-module: TransactionCompleted ✅
    ├── Dispute/
    │   └── Dispute.Application/
    │       └── EventHandlers/            # Cross-module: FraudDetected ✅
    └── ... (diğer modüller)
```

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

### Yeni Domain Event Eklemek

**1. Local Event tanımla (Domain katmanı):**
```csharp
// Card.Domain/Events/CardApplicationEvents.cs
public class CardApplicationApprovedEvent : DomainEvent
{
    public Guid ApplicationId { get; }
    public CardApplicationApprovedEvent(Guid applicationId)
    {
        ApplicationId = applicationId;
    }
}
```

**2. Aggregate'de fırlat:**
```csharp
// Card.Domain/Entities/CardApplication.cs
public Result Approve()
{
    Status = CardApplicationStatus.Approved;
    AddDomainEvent(new CardApplicationApprovedEvent(Id));       // Local
    AddDomainEvent(new CardApplicationApprovedIntegrationEvent(...)); // Cross-module
    return Result.Success();
}
```

**3. Handler yaz (Application katmanı):**
```csharp
// Card.Application/EventHandlers/CardApplicationApprovedEventHandler.cs
public class CardApplicationApprovedEventHandler : INotificationHandler<CardApplicationApprovedEvent>
{
    public async Task Handle(CardApplicationApprovedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation("✅ [Card] Başvuru onaylandı - {ApplicationId}", notification.ApplicationId);
        await Task.CompletedTask;
    }
}
```

**4. Cross-module event gerekiyorsa Integration Event ekle:**
```csharp
// Shared/Events/IntegrationEvents.cs
public class CardApplicationApprovedIntegrationEvent : DomainEvent { ... }

// OtherModule.Application/EventHandlers/
public class CardApplicationApprovedIntegrationEventHandler
    : INotificationHandler<CardApplicationApprovedIntegrationEvent> { ... }
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
        // Validation → Business logic → Save (otomatik event dispatch)
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
        // Read from database → Map to DTO → Return
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

### Domain Event Tetiklenmiyor

**Kontrol listesi:**
1. `MediatorExtensions.cs` — `Shared/Extensions/` altında var mı?
2. DbContext constructor'da `IMediator? mediator` parametresi var mı?
3. `SaveChangesAsync`'te `await _mediator.DispatchDomainEventsAsync(this)` çağrılıyor mu?
4. Design-time factory'lerde constructor'a `null` geçildiğinden emin ol

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

- [x] **Domain Event Infrastructure** ✅
  - [x] MediatorExtensions dispatcher
  - [x] DbContext'lere IMediator injection
  - [x] Same-module event handlers (Card, Merchant, Transaction)
  - [x] Integration events (Shared)
  - [x] Cross-module event handlers (BKM, HSM, BulkCardPrint, Campaign, Fee, Accounting, Dispute)
  - [x] Test senaryoları doğrulandı

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

**Son Güncelleme:** 31 Ocak 2026  
**Versiyon:** 1.5.0  
**Tamamlanma:** %92 (17/19 modül + Domain Events)  
**Durum:** ✅ Production-Ready