# Card Merchant System

Bankacılık sektörü için kapsamlı Kart ve Üye İşyeri Yönetim Sistemi.

---

## 🚀 Hızlı Başlangıç

### Gereksinimler

- .NET 8 SDK
- SQL Server (LocalDB veya Express)
- Redis (opsiyonel - Transaction modülü için)
- Visual Studio 2022

### Kurulum

1. Repository'yi klonla
2. `appsettings.json` dosyasındaki connection string'i güncelle
3. Migration'ları uygula:
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

4. Projeyi çalıştır: `F5` veya `dotnet run`
5. Swagger: `https://localhost:7202/swagger`

### Test Kullanıcıları

| Username | Password | Rol |
|----------|----------|-----|
| admin | Admin123! | Admin |
| callcenter | Test123! | CallCenterAgent |

---

## 📊 Proje Durumu

**Backend Modülleri:** 17/19 tamamlandı (%89)
**Mimari Geliştirmeler:** 2/5 tamamlandı (%40)

> Detaylı durum için: [PROJECT_STATUS.md](PROJECT_STATUS.md)

---

## 🏗️ Mimari
```
┌─────────────────────────────────────────────────────────────┐
│                        API Layer                            │
│              (Controllers, Middleware, Auth)                │
├─────────────────────────────────────────────────────────────┤
│                    Application Layer                        │
│               (Commands, Queries, DTOs)                     │
├─────────────────────────────────────────────────────────────┤
│                      Domain Layer                           │
│            (Entities, Enums, Repositories)                  │
├─────────────────────────────────────────────────────────────┤
│                   Infrastructure Layer                      │
│           (DbContext, EF Configurations)                    │
└─────────────────────────────────────────────────────────────┘
```

### Kullanılan Pattern'ler

- **Clean Architecture** - Katmanlı mimari
- **CQRS** - Command Query Responsibility Segregation
- **Mediator** - MediatR ile request/handler
- **Repository** - Veri erişim soyutlama
- **Enumeration** - Type-safe enum'lar

---

## 📦 Modüller

### Tamamlanan (17)

| Modül | Açıklama |
|-------|----------|
| Card | Kart yönetimi, başvuru, limit |
| Merchant | Üye işyeri, terminal, komisyon |
| Transaction | İşlem, provizyon, takas |
| Dispute | İtiraz yönetimi |
| Campaign | Kampanya, kural motoru |
| BKM | Switch entegrasyonu |
| HSM | Güvenlik modülü |
| Fee | Ücret yönetimi |
| Statement | Ekstre yönetimi |
| Accounting | Muhasebe entegrasyonu |
| MerchantReport | Üye işyeri raporlama |
| MerchantSettlement | Üye işyeri takas |
| BulkCardPrint | Toplu kart basım |
| RegulatoryReporting | Yasal raporlama (BDDK/TCMB) |
| Courier | Kurye entegrasyonu |
| EarlyBlockResolution | Erken bloke çözüm |
| WorkOrder | İş emri yönetimi |

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
| ORM | Entity Framework Core 8 |
| Veritabanı | SQL Server |
| Cache | Redis |
| Jobs | Hangfire |
| Auth | JWT + BCrypt |
| Validation | FluentValidation |
| Mediator | MediatR 12 |

---

## 📁 Klasör Yapısı
```
CardMerchantSystem/
├── src/
│   ├── CardMerchantSystem.API/
│   ├── CardMerchantSystem.Shared/
│   └── Modules/
│       ├── Card/
│       ├── Merchant/
│       ├── Transaction/
│       └── ... (17 modül)
└── tests/
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

---

## 📄 Lisans

Bu proje eğitim amaçlıdır.