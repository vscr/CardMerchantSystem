# Card Merchant System

Bankacılık sektörü için kapsamlı Kart ve Üye İşyeri Yönetim Sistemi.

---

## 🚀 Hızlı Başlangıç

### Gereksinimler

- .NET 8 SDK
- SQL Server (LocalDB veya Express)
- Docker Desktop (Elasticsearch + Kibana için)
- Visual Studio 2022

### Kurulum

1. Repository'yi klonla
2. `appsettings.json` dosyasındaki connection string'i güncelle
3. Docker servislerini başlat:
```bash
docker-compose up -d
```

4. Migration'ları uygula:
```powershell
# Tüm DbContext'ler için
Update-Database -Context CardDbContext
Update-Database -Context MerchantDbContext
Update-Database -Context TransactionDbContext
Update-Database -Context DisputeDbContext
Update-Database -Context CampaignDbContext
Update-Database -Context BKMDbContext
Update-Database -Context HSMDbContext
Update-Database -Context FeeDbContext
Update-Database -Context StatementDbContext
Update-Database -Context AccountingDbContext
Update-Database -Context MerchantReportDbContext
Update-Database -Context MerchantSettlementDbContext
Update-Database -Context BulkCardPrintDbContext
Update-Database -Context RegulatoryReportingDbContext
Update-Database -Context CourierDbContext
Update-Database -Context EarlyBlockResolutionDbContext
Update-Database -Context WorkOrderDbContext
Update-Database -Context AuthDbContext
```

5. Projeyi çalıştır: `F5` veya `dotnet run`
6. Swagger: `https://localhost:7202/swagger`
7. Kibana (Logs): `http://localhost:5601`

### Test Kullanıcıları

| Username | Password | Rol |
|----------|----------|-----|
| admin | Admin123! | Admin |
| callcenter | Test123! | CallCenterAgent |

---

## 📊 Proje Durumu

**Backend Modülleri:** 17/19 tamamlandı (%89)
**Mimari Geliştirmeler:** 4/6 tamamlandı (%67)

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
│     (DbContext, EF Core, Dapper, External Services)         │
└─────────────────────────────────────────────────────────────┘
```

### Kullanılan Pattern'ler

- **Clean Architecture** - Katmanlı mimari
- **CQRS** - Command Query Responsibility Segregation (EF Core + Dapper)
- **Mediator** - MediatR ile request/handler
- **Repository** - Veri erişim soyutlama
- **Enumeration** - Type-safe enum'lar

---

## 📦 Modüller

### Tamamlanan (17)

| Modül | Açıklama | Dapper |
|-------|----------|--------|
| Card | Kart yönetimi, başvuru, limit | - |
| Merchant | Üye işyeri, terminal, komisyon | ✅ |
| Transaction | İşlem, provizyon, takas | - |
| Dispute | İtiraz yönetimi | - |
| Campaign | Kampanya, kural motoru | - |
| BKM | Switch entegrasyonu | - |
| HSM | Güvenlik modülü | - |
| Fee | Ücret yönetimi | - |
| Statement | Ekstre yönetimi | - |
| Accounting | Muhasebe entegrasyonu | - |
| MerchantReport | Üye işyeri raporlama | - |
| MerchantSettlement | Üye işyeri takas | - |
| BulkCardPrint | Toplu kart basım | - |
| RegulatoryReporting | Yasal raporlama (BDDK/TCMB) | - |
| Courier | Kurye entegrasyonu | - |
| EarlyBlockResolution | Erken bloke çözüm | - |
| WorkOrder | İş emri yönetimi | - |

### Planlanan (2)

| Modül | Açıklama | Öncelik |
|-------|----------|---------|
| InstantCardPrint | Anında kart basım | Düşük |
| Inventory | Envanter yönetimi | Düşük |

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
| ORM | Entity Framework Core 8 + Dapper |
| Veritabanı | SQL Server |
| Cache | Redis |
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
- **Information**: Normal işlem akışı
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
├── docker-compose.yml                    # Elasticsearch + Kibana
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
│   │   └── Dapper/
│   │       ├── IDapperContext.cs
│   │       ├── BaseDapperRepository.cs
│   │       ├── Extensions/
│   │       └── Models/
│   └── ...
└── Modules/
    ├── Merchant/
    │   ├── Merchant.Domain/
    │   │   ├── ReadModels/              # Dapper DTOs
    │   │   └── ...
    │   ├── Merchant.Application/
    │   └── Merchant.Infrastructure/
    │       ├── Data/
    │       │   └── Dapper/
    │       │       └── MerchantDapperContext.cs
    │       └── Repositories/
    │           └── MerchantDapperRepository.cs
    └── ... (17 modül)
```

---

## 📝 Geliştirici Notları

### Controller Oluşturma
```csharp
[Authorize(Policy = Policies.XxxManagement)]
public class XxxController : ApiControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<XxxDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetXxxByIdQuery(id), ct);
        return Ok(HandleNotFound(result, "Xxx", id));
    }

    [HttpPost]
    public async Task<ActionResult<XxxDto>> Create([FromBody] CreateXxxDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateXxxCommand(dto), ct);
        var value = HandleResult(result);
        return CreatedResponse(nameof(GetById), new { id = value.Id }, value);
    }
}
```

### Exception Kullanımı
```csharp
throw new NotFoundException("Entity", id);
throw new BusinessRuleException("Hata mesajı");
throw new ConflictException("Kayıt zaten mevcut");
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

---

## 🐳 Docker Compose

### Servisler
```yaml
# Elasticsearch + Kibana
docker-compose up -d

# Servisler:
- Elasticsearch: http://localhost:9200
- Kibana: http://localhost:5601
```

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
```

---

## 📊 Monitoring ve Analiz

### Kibana'da Log Analizi

1. **Discover**: http://localhost:5601
2. Index pattern: `cardmerchant-logs-*`
3. **Filtreler:**
```
fields.RequestPath: "/api/merchant*"
level: "Error"
fields.Elapsed > 1000
fields.Username: "admin"
```

### Performance Metrikleri

- Request count
- Response time distribution
- Error rate
- Slow queries (>100ms)
- User activity

---

## 📄 Lisans

Bu proje eğitim amaçlıdır.