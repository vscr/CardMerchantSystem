# Card Merchant System

Bankacılık sektörü için **production-ready** Kart ve Üye İşyeri Yönetim Sistemi.

---

## 🎯 Proje Özeti

**Tamamlanma:** %96 (17/19 modül + Domain Events + Performance + Audit Trail ✅)  
**Mimari:** Clean Architecture + DDD + CQRS + Event-Driven  
**Database:** SQL Server + PostgreSQL (Multi-DB)  
**Resilience:** Polly + Rate Limiting + Redis Cache  
**Performance:** Dapper + Snapshot Isolation + Connection Pooling  
**Monitoring:** Serilog + Elasticsearch + Kibana  
**Audit:** Entity Change Tracking + Dapper Writer

---

## 📦 Modül Durumu

### ✅ Tamamlanan (17)

| Modül | Açıklama | Özel Özellik |
|-------|----------|--------------|
| **Card** | Kart başvuru, tahsis, limit | 🌟 Domain Events (local + integration) |
| **Merchant** | Üye işyeri, terminal yönetimi | 🌟 Multi-DB (SQL Server + PostgreSQL)<br>🌟 CQRS (EF Core + Dapper)<br>🌟 Domain Events (local + integration)<br>🔍 Audit Trail |
| **Transaction** | İşlem, provizyon, LKS | 🌟 Polly Resilience<br>🌟 Redis Cache<br>🌟 Rate Limiting<br>🌟 Domain Events (local + integration)<br>🚀 **Dapper High-Performance Read**<br>🚀 **150 TPS Load Tested**<br>🔍 Audit Trail<br>🛡️ **Refund Duplicate Prevention** |
| **Dispute** | İtiraz yönetimi | 🌟 Cross-module: FraudDetected dinler<br>🔍 Audit Trail |
| **Campaign** | Kampanya, kural motoru | 🌟 Cross-module: TransactionCompleted dinler<br>🎯 **Post-Transaction Benefit Calculation**<br>🔍 Audit Trail |
| **BKM** | Switch entegrasyonu | 🌟 Cross-module: TerminalActivated dinler<br>🔍 Audit Trail |
| **HSM** | Güvenlik modülü (Thales, Gemalto) | 🌟 Cross-module: CardApplicationApproved + TerminalActivated dinler<br>🔍 Audit Trail |
| **Fee** | Ücret, aidat, tarife yönetimi | 🌟 Cross-module: TransactionCompleted dinler<br>🔍 Audit Trail |
| **Statement** | Ekstre yönetimi | Document generation |
| **Accounting** | Muhasebe entegrasyonu | 🌟 Cross-module: TransactionCompleted dinler |
| **MerchantReport** | Üye işyeri raporlama (FTP, mail) | Scheduled reports<br>🔍 Audit Trail |
| **MerchantSettlement** | Üye işyeri takas | Payment processing<br>🔍 Audit Trail |
| **BulkCardPrint** | Toplu kart basım (Bileşim, Austria) | 🌟 Cross-module: CardApplicationApproved dinler<br>🔍 Audit Trail |
| **RegulatoryReporting** | Yasal raporlama (BDDK, TCMB) | Compliance |
| **Courier** | Kurye entegrasyonu (Kuryenet) | Shipment tracking<br>🔍 Audit Trail |
| **EarlyBlockResolution** | Erken bloke çözüm | - |
| **WorkOrder** | İş emri yönetimi (Beko, Ingenico, Teknoser) | - |

### ✅ Teknik Altyapı

| Özellik | Durum | Açıklama |
|---------|-------|----------|
| **Audit Trail** | ✅ | Entity değişiklik takibi - Merkezi Dapper Writer |
| **Health Checks** | ⏳ | Planlandı |
| **Günsonu Jobs** | ⏳ | Planlandı |

### ⏳ Planlanmış (2)

| Modül | Öncelik |
|-------|---------|
| **InstantCardPrint** (Evolis Primacy) | Düşük |
| **Inventory** (Envanter yönetimi) | Düşük |

---

## 🔍 Audit Trail

### Genel Bakış

Tüm entity değişiklikleri (Insert, Update, Delete, SoftDelete) otomatik olarak `audit.AuditLogs` tablosuna kaydedilir.

**Kaydedilen Bilgiler:**
- **Kim** yaptı (UserId, UserName)
- **Ne zaman** yaptı (Timestamp)
- **Hangi entity** üzerinde (EntityName, EntityId)
- **Ne değiştirdi** (OldValues, NewValues, ChangedColumns)
- **Nereden** yaptı (IpAddress, CorrelationId)

### Mimari (Yaklaşım 2 - Merkezi Dapper Writer)

```
┌──────────────────┐     ┌─────────────────────────┐     ┌──────────────┐
│  DbContext       │────▶│  SaveChangesAsync()     │────▶│   Database   │
│  (Any Module)    │     └───────────┬─────────────┘     └──────────────┘
└──────────────────┘                 │
                         ┌───────────▼─────────────┐
                         │  AuditSaveChangesInterceptor │
                         └───────────┬─────────────┘
                                     │
                         ┌───────────▼─────────────┐
                         │   IAuditLogWriter       │
                         │   (Dapper - Direct SQL) │
                         └───────────┬─────────────┘
                                     │
                         ┌───────────▼─────────────┐
                         │   audit.AuditLogs       │
                         │   (Merkezi Tablo)       │
                         └─────────────────────────┘
```

### Avantajlar

| Özellik | Açıklama |
|---------|----------|
| **DbContext'ten bağımsız** | Her modülün DbContext'ine AuditLogs eklemeye gerek yok |
| **Dapper ile doğrudan SQL** | Performanslı, ayrı connection kullanır |
| **Fire-and-forget** | Ana işlemi bloke etmez |
| **Multi-database** | SQL Server ve PostgreSQL destekler |
| **Smart Enum desteği** | Cycle olmadan serialize eder |

### API Endpoints

| Endpoint | Açıklama |
|----------|----------|
| `GET /api/audit` | Sayfalı audit log listesi |
| `GET /api/audit/{id}` | ID ile audit log getir |
| `GET /api/audit/entity/{entityName}/{entityId}` | Entity geçmişi |
| `GET /api/audit/user/{userId}` | Kullanıcı aksiyonları |
| `GET /api/audit/date-range` | Tarih aralığı sorgusu |
| `GET /api/audit/entity-types` | Desteklenen entity tipleri |
| `GET /api/audit/action-types` | Aksiyon tipleri |

### Kurulum

**1. Migration:**
```powershell
Add-Migration InitAuditModule -Context AuditDbContext -OutputDir Audit/Persistence/Migrations/SqlServer -Project CardMerchantSystem.Shared -StartupProject CardMerchantSystem.API

Update-Database -Context AuditDbContext -Project CardMerchantSystem.Shared -StartupProject CardMerchantSystem.API
```

**2. Program.cs:**
```csharp
// DİĞER SERVİSLERDEN ÖNCE
builder.Services.AddAuditTrail();
```

**3. Her modülün DI'ına interceptor ekle:**
```csharp
services.AddDbContext<XxxDbContext>((sp, options) =>
{
    options.ConfigureDatabase(provider, connectionString);
    
    var auditInterceptor = sp.GetService<AuditSaveChangesInterceptor>();
    if (auditInterceptor != null)
        options.AddInterceptors(auditInterceptor);
});
```

---

## 🎯 Campaign Module (Post-Transaction)

### Kampanya Uygulama Akışı

İşlem tamamlandıktan sonra (`TransactionCompletedIntegrationEvent`) kampanya kazanımları hesaplanır:

```
Transaction Approve
       │
       ▼
TransactionCompletedIntegrationEvent
       │
       ▼
CampaignApplicationService.ApplyEligibleCampaignsAsync()
       │
       ├─► Aktif kampanyaları bul
       ├─► Müşteri kullanım limiti kontrolü
       ├─► Kural değerlendirme (MCC, BIN, Amount)
       ├─► İndirim/Puan/Cashback hesapla
       └─► CampaignUsage kaydet
```

### Kural Tipleri

| RuleType | Açıklama | Örnek |
|----------|----------|-------|
| `MinAmount` | Minimum işlem tutarı | `>= 100` |
| `MaxAmount` | Maksimum işlem tutarı | `<= 5000` |
| `MCC` | MCC kategorisi | `IN 5411,5412` (market) |
| `CardBIN` | Kart BIN kontrolü | `IN 411111,422222` |

### Kampanya Tipleri

| Tip | Açıklama |
|-----|----------|
| **Discount** | Anlık indirim |
| **Cashback** | Para iadesi |
| **Points** | Puan kazanım |
| **BonusPoints** | Ekstra puan |
| **Installment** | Taksit |
| **FreeShipping** | Ücretsiz kargo |

### Örnek Log Çıktısı

```
🎯 [Campaign] Kampanya hesaplama başladı - TransactionId: abc123, Amount: 500.00
  📌 CMP2024010112345 uygulanamadı: Minimum işlem tutarı 1000 TL olmalı
  📌 CMP2024010167890: Discount - Discount=25.00, Points=0, Cashback=0
  📌 CMP2024010199999: Points - Discount=0, Points=500, Cashback=0
✅ [Campaign] 2 kampanya uygulandı - TotalDiscount: 25.00, TotalPoints: 500
```

---

## 🛡️ Transaction Refund Validation

### İade Kontrolü

Aynı işlemin birden fazla kez veya tutarını aşan şekilde iade edilmesi engellenir:

| Senaryo | Davranış |
|---------|----------|
| Tam iade yapılmış işlem | ❌ `REFUND_ALREADY_PROCESSED` |
| Kısmi iade + kalan aşan tutar | ❌ `REFUND_AMOUNT_EXCEEDED` |
| Kısmi iade + kalan içinde | ✅ İzin verilir |

### Örnek Akış

```
Orijinal İşlem: 100 TRY

İade #1: 30 TRY  → ✅ "Kısmi iade onaylandı. Kalan: 70 TRY"
İade #2: 50 TRY  → ✅ "Kısmi iade onaylandı. Kalan: 20 TRY"
İade #3: 30 TRY  → ❌ "İade edilebilir: 20 TRY. Daha önce 80 TRY iade edilmiş."
İade #4: 20 TRY  → ✅ "Tam iade işlemi onaylandı"
İade #5: 10 TRY  → ❌ "Bu işlemin tamamı zaten iade edilmiş"
```

---

## 🚀 Performance & Load Testing

### Load Test Sonuçları ✅

| Metrik | Değer |
|--------|-------|
| **Süre** | 600 saniye (10 dakika) |
| **TPS** | 150 req/sec |
| **Concurrency** | 220 |
| **Toplam İşlem** | ~90,000 transaction |
| **Mix** | Sale %92, Refund %6, Cancel %2 |
| **Warmup** | 3,000 istek |

### Türkiye Bankacılık Karşılaştırması

| Sistem | TPS |
|--------|-----|
| **Bu Proje (test)** | **150 TPS ✅** |
| Orta ölçekli banka (normal) | 100-300 TPS |
| Orta ölçekli banka (peak) | 500-1,000 TPS |
| Büyük banka | 2,000-5,000 TPS |
| BKM/Troy | 15,000+ TPS |

### Performance Optimizasyonları

| Optimizasyon | Açıklama | Etki |
|--------------|----------|------|
| **Snapshot Isolation** | `READ_COMMITTED_SNAPSHOT ON` | Read/Write lock yok |
| **Connection Pooling** | `Max Pool Size=200` | Yeterli DB bağlantısı |
| **Dapper Read Repository** | `WITH (NOLOCK)` raw SQL | 5-15x hızlı read |
| **Composite Indexes** | 7 optimized index | %80 query time azalma |
| **AsNoTracking** | EF Core read queries | Memory optimization |
| **Audit Fire-and-Forget** | Async audit yazma | Ana işlemi bloke etmez |

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
│    Services (CampaignApplicationService, etc.)             │
├─────────────────────────────────────────────────────────────┤
│                     Domain Layer                            │
│   Aggregates, Entities, Value Objects, Smart Enums,        │
│   Domain Events, Integration Events, Business Rules        │
├─────────────────────────────────────────────────────────────┤
│                 Infrastructure Layer                        │
│  EF Core (Write), Dapper (Read), PostgreSQL, SQL Server    │
│  Redis Cache, Polly Resilience, External APIs              │
├─────────────────────────────────────────────────────────────┤
│                    Shared Layer                             │
│  Audit Trail, Kernel, Events, Data Extensions              │
└─────────────────────────────────────────────────────────────┘
```

### Kullanılan Pattern'ler

| Pattern | Modül | Açıklama |
|---------|-------|----------|
| **Clean Architecture** | Tümü | Katmanlı mimari, dependency inversion |
| **Domain-Driven Design** | Tümü | Aggregates, Value Objects, Smart Enums |
| **CQRS** | Merchant, Transaction | EF Core (Write) + Dapper (Read) |
| **Multi-Database** | Merchant | SQL Server + PostgreSQL support |
| **Resilience** | Transaction | Polly (Retry, Circuit Breaker, Timeout) |
| **Repository** | Tümü | Data access abstraction |
| **Mediator** | Tümü | MediatR (commands/queries/events) |
| **Domain Events** | Card, Merchant, Transaction | ✅ Local events + Cross-module integration events |
| **Interceptor** | Audit | EF Core SaveChanges interceptor |

---

## 🌟 Öne Çıkan Özellikler

### 1. Domain Events (Event-Driven Architecture) ✅

**İki katmanlı event sistem:**

```
Aggregate
    ├── Local Domain Events       → Same-module handler'lar (logging, audit)
    └── Integration Events        → Cross-module handler'lar (iş akışı)
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
🔍 [Audit] Terminal değişikliği kaydedildi  ← Audit Trail
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
🎯 [Campaign] Kampanya kazanımları hesaplanıyor  ← Integration event
💰 [Fee] Komisyon hesaplıyor                ← Integration event
📊 [Accounting] Muhasebe kayıt oluşturuyor  ← Integration event
🔍 [Audit] Transaction değişikliği kaydedildi  ← Audit Trail
```

---

### 2. Multi-Database Support (Merchant Modülü)

**Tek kod tabanı, iki veritabanı:**

```
MerchantDbContextBase (Abstract)
    ├── MerchantDbContext (SQL Server)
    └── MerchantDbContext_Pg (PostgreSQL)
```

**Provider değiştirme:**
```json
{
  "Database": {
    "Provider": "PostgreSql"  // veya "SqlServer"
  }
}
```

---

### 3. CQRS Pattern

**Write (EF Core):**
```csharp
// Domain-driven, business logic with tracking
var merchant = MerchantAggregate.Create(...);
await _repository.AddAsync(merchant);
await _repository.SaveChangesAsync(); // Domain events + Audit Trail
```

**Read (Dapper):**
```csharp
// High-performance, optimized queries
var transactions = await _dapperRepository.GetPagedAsync(filter);
```

---

### 4. Resilience Patterns (Transaction Modülü)

**Polly Policies:**
- **Retry:** 3 attempts with exponential backoff
- **Circuit Breaker:** 5 failures → 30s break
- **Timeout:** 30s per request

---

## 🚀 Hızlı Başlangıç

### 1. Gereksinimler

- .NET 8 SDK
- Docker Desktop
- Visual Studio 2022 / VS Code
- SQL Server (LocalDB veya Express)
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
- Redis: `localhost:6379`

### 3. Database Konfigürasyonu

**appsettings.json:**
```json
{
  "Database": {
    "Provider": "SqlServer",
    "SqlServerConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;Max Pool Size=200;Min Pool Size=20;Connection Timeout=30;",
    "PostgreSqlConnection": "Host=localhost;Port=5432;Database=cardmerchantdb;Username=postgres;Password=postgres;"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;Max Pool Size=200;Min Pool Size=20;Connection Timeout=30;"
  }
}
```

### 4. Database Performance Optimization

**Snapshot Isolation (ÖNEMLİ!):**
```sql
-- SQL Server'da çalıştır - Read/Write lock'larını önler
ALTER DATABASE CardMerchantDb SET ALLOW_SNAPSHOT_ISOLATION ON;
ALTER DATABASE CardMerchantDb SET READ_COMMITTED_SNAPSHOT ON;
```

### 5. Migration'ları Uygula

```powershell
# Package Manager Console

# Auth Module
Update-Database -Context AuthDbContext -Project CardMerchantSystem.API -StartupProject CardMerchantSystem.API

# Audit Module (YENİ)
Update-Database -Context AuditDbContext -Project CardMerchantSystem.Shared -StartupProject CardMerchantSystem.API

# Card Module
Update-Database -Context CardDbContext -Project Card.Infrastructure -StartupProject CardMerchantSystem.API

# Merchant Module
Update-Database -Context MerchantDbContext -Project Merchant.Infrastructure -StartupProject CardMerchantSystem.API

# Transaction Module
Update-Database -Context TransactionDbContext -Project Transaction.Infrastructure -StartupProject CardMerchantSystem.API

# Campaign Module
Update-Database -Context CampaignDbContext -Project Campaign.Infrastructure -StartupProject CardMerchantSystem.API

# ... (diğer modüller için tekrarla)
```

### 6. Uygulamayı Çalıştır

```bash
dotnet run --project CardMerchantSystem.API
```

**Erişim:**
- Swagger: `https://localhost:7202/swagger`
- Kibana: `http://localhost:5601`

### 7. Test Kullanıcıları

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
| **ORM (Read)** | Dapper 2.1 |
| **Database** | SQL Server + PostgreSQL |
| **Cache** | Redis |
| **Mediator** | MediatR 12 |
| **Resilience** | Polly 8.x (Retry, Circuit Breaker, Timeout) |
| **Rate Limiting** | .NET 8 Built-in (Configurable) |
| **Validation** | FluentValidation |
| **Auth** | JWT + BCrypt |
| **Jobs** | Hangfire |
| **Logging** | Serilog + Elasticsearch + Kibana |
| **Containerization** | Docker Compose |
| **Audit** | EF Core Interceptor + Dapper |

---

## 🚀 High-Performance Transaction Endpoints

### Dapper-Based Fast Endpoints

Transaction modülünde **yüksek performanslı read endpoint'leri** mevcuttur:

| Endpoint | Açıklama | Performans |
|----------|----------|------------|
| `GET /api/transactions/fast` | Sayfalı liste | 5x hızlı |
| `GET /api/transactions/fast/{id}` | ID ile getir | 3x hızlı |
| `GET /api/transactions/fast/health` | Health check (no auth) | - |
| `GET /api/transactions/fast/stats` | Dashboard istatistikleri | 15x hızlı |
| `GET /api/transactions/fast/card-daily-total/{card}` | Kart günlük limit | 10x hızlı |
| `GET /api/transactions/fast/stats/daily-trend` | Günlük trend | - |
| `GET /api/transactions/fast/stats/top-merchants` | Top merchant'lar | - |

### Performans Karşılaştırması

| İşlem | EF Core | Dapper | İyileşme |
|-------|---------|--------|----------|
| GetPaged (10K kayıt) | ~250ms | ~45ms | **5.5x** |
| GetStats (1M kayıt) | ~1200ms | ~80ms | **15x** |
| CardDailyTotal | ~150ms | ~15ms | **10x** |
| GetById | ~25ms | ~8ms | **3x** |

---

## ⚡ Rate Limiting

### Konfigürasyon

**appsettings.json:**
```json
{
  "RateLimiting": {
    "Enabled": true,
    "Global": { "PermitLimit": 5000, "WindowSeconds": 1 },
    "Strict": { "PermitLimit": 10, "WindowMinutes": 5 },
    "Standard": { "PermitLimit": 300, "WindowMinutes": 1 },
    "Transaction": { "PermitLimit": 1000, "WindowSeconds": 1 }
  }
}
```

### Policy'ler

| Policy | Limit | Kullanım |
|--------|-------|----------|
| **Global** | 5000 TPS/IP | DDoS koruması |
| **Auth/Strict** | 10/5dk/IP | Login brute force |
| **Transaction** | 1000 TPS/IP | İşlem endpoint'leri |
| **Standard** | 300/dk/User | CRUD işlemleri |
| **Relaxed** | 1000/dk/IP | Read endpoint'ler |
| **Report** | 20/dk/User | Ağır raporlar |

---

## 📁 Proje Yapısı

```
CardMerchantSystem/
├── docker-compose.yml
├── CardMerchantSystem.sln
├── CardMerchantSystem.API/
│   ├── Controllers/
│   │   ├── TransactionsController.cs      # EF Core endpoints
│   │   ├── TransactionsFastController.cs  # Dapper endpoints 🚀
│   │   └── AuditController.cs             # Audit Trail API 🔍
│   ├── Configuration/
│   │   └── RateLimitingConfiguration.cs
│   └── Auth/
├── CardMerchantSystem.Shared/
│   ├── Kernel/
│   ├── Events/
│   ├── Data/
│   └── Audit/                             # 🔍 Audit Trail
│       ├── Entities/
│       │   └── AuditLog.cs
│       ├── Enums/
│       │   └── AuditActionType.cs
│       ├── Interceptors/
│       │   └── AuditSaveChangesInterceptor.cs
│       ├── Services/
│       │   ├── IAuditService.cs
│       │   └── AuditService.cs
│       ├── Persistence/
│       │   ├── AuditDbContext.cs
│       │   └── AuditDbContext_Pg.cs
│       ├── AuditEntry.cs
│       ├── AuditContextAccessor.cs
│       ├── IAuditLogWriter.cs
│       ├── AuditLogWriter.cs
│       └── DependencyInjection.cs
└── Modules/
    ├── Transaction/
    │   ├── Transaction.Application/
    │   │   └── Commands/
    │   │       └── RefundTransactionCommand.cs  # 🛡️ Duplicate prevention
    │   └── Transaction.Infrastructure/
    │       ├── Dapper/                    # 🚀 High-performance read
    │       └── Repositories/
    ├── Campaign/
    │   └── Campaign.Application/
    │       ├── Services/
    │       │   ├── ICampaignApplicationService.cs  # 🎯 Post-tx service
    │       │   └── CampaignApplicationService.cs
    │       └── EventHandlers/
    │           └── TransactionCompletedIntegrationEventHandler.cs
    ├── Merchant/
    ├── Card/
    └── ... (diğer modüller)
```

---

## 🔐 Güvenlik & Yetkilendirme

### Roller

| Rol | Açıklama |
|-----|----------|
| **Admin** | Tüm yetkiler |
| **CardOperator** | Kart operasyonları |
| **MerchantOperator** | Üye işyeri operasyonları |
| **FinanceOperator** | Finans işlemleri |
| **ComplianceOfficer** | Yasal raporlama + Audit erişimi |
| **CallCenterAgent** | Çağrı merkezi |
| **Viewer** | Sadece görüntüleme |

### Audit API Erişimi

Audit endpoint'lerine sadece `Admin` ve `ComplianceOfficer` rolleri erişebilir.

---

## 🔧 Sorun Giderme

### Performance Sorunları

**1. Read endpoint'ler yavaş/timeout:**
```sql
-- Snapshot Isolation açık mı kontrol et
SELECT name, snapshot_isolation_state_desc, is_read_committed_snapshot_on
FROM sys.databases WHERE name = 'CardMerchantDb';

-- Açık değilse:
ALTER DATABASE CardMerchantDb SET READ_COMMITTED_SNAPSHOT ON;
```

**2. Connection pool tükeniyor:**
```json
// appsettings.json - Pool size artır
"ConnectionStrings": {
    "DefaultConnection": "...;Max Pool Size=200;Min Pool Size=20;"
}
```

**3. Rate limit hatası (429):**
```json
// Test için kapat
"RateLimiting": { "Enabled": false }
```

### Audit Trail Sorunları

**1. Audit logları yazılmıyor:**
- `AddAuditTrail()` Program.cs'de diğer servislerden önce çağrıldı mı?
- DbContext'e interceptor eklendi mi?
- `audit` schema ve `AuditLogs` tablosu var mı?

**2. JSON serialize hatası (cycle):**
- Smart Enum'lar `SimplifyValue()` ile basitleştiriliyor
- Kompleks tipler `ToString()` ile kaydediliyor

---

## 🚧 Gelecek Geliştirmeler

### Kısa Vadeli

- [ ] Health Checks (DB, Redis, External APIs)
- [ ] Günsonu Jobs (Settlement, Reconciliation)
- [ ] InstantCardPrint Module
- [ ] Inventory Module

### Orta Vadeli

- [ ] Event Sourcing (Dispute modülü)
- [ ] GraphQL API
- [ ] Real-time Notifications (SignalR)
- [ ] Audit Log Retention Policy

### Uzun Vadeli

- [ ] Microservices Migration
- [ ] Saga Pattern
- [ ] gRPC Integration

---

## 📄 Lisans

Bu proje eğitim amaçlıdır ve MIT lisansı altında paylaşılmaktadır.

---

**Son Güncelleme:** 07 Şubat 2026  
**Versiyon:** 1.7.0  
**Tamamlanma:** %96 (17/19 modül + Domain Events + Performance + Audit Trail)  
**Durum:** ✅ Production-Ready | 🚀 150 TPS Load Tested | 🔍 Full Audit Trail