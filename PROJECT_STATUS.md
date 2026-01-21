# Card Merchant System - Proje Durumu

> Son Güncelleme: 21 Ocak 2026

---

## 📊 Genel Durum

| Kategori | Tamamlanan | Toplam | Yüzde |
|----------|------------|--------|-------|
| Backend Modülleri | 17 | 19 | %89 |
| Mimari Geliştirmeler | 2 | 5 | %40 |

---

## ✅ Tamamlanan Backend Modülleri (17/19)

| # | Modül | Açıklama | DbContext |
|---|-------|----------|-----------|
| 1 | Card | Kart yönetimi, başvuru, limit | CardDbContext |
| 2 | Merchant | Üye işyeri, terminal, komisyon | MerchantDbContext |
| 3 | Transaction | İşlem, provizyon, takas | TransactionDbContext |
| 4 | Dispute | İtiraz yönetimi | DisputeDbContext |
| 5 | Campaign | Kampanya, kural motoru | CampaignDbContext |
| 6 | BKM | Switch entegrasyonu | BKMDbContext |
| 7 | HSM | Güvenlik modülü | HSMDbContext |
| 8 | Fee | Ücret yönetimi | FeeDbContext |
| 9 | Statement | Ekstre yönetimi | StatementDbContext |
| 10 | Accounting | Muhasebe entegrasyonu | AccountingDbContext |
| 11 | MerchantReport | Üye işyeri raporlama | MerchantReportDbContext |
| 12 | MerchantSettlement | Üye işyeri takas | MerchantSettlementDbContext |
| 13 | BulkCardPrint | Toplu kart basım | BulkCardPrintDbContext |
| 14 | RegulatoryReporting | Yasal raporlama (BDDK/TCMB) | RegulatoryReportingDbContext |
| 15 | Courier | Kurye entegrasyonu | CourierDbContext |
| 16 | EarlyBlockResolution | Erken bloke çözüm, fraud | EarlyBlockResolutionDbContext |
| 17 | WorkOrder | İş emri yönetimi | WorkOrderDbContext |

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
| 3 | Validation Pipeline | ⏳ | MediatR FluentValidation behavior |
| 4 | Audit Trail | ⏳ | Entity değişiklik takibi |
| 5 | Health Checks | ⏳ | API ve DB sağlık kontrolü |

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
| ORM | Entity Framework Core 8 |
| Veritabanı | SQL Server (LocalDB) |
| Cache | Redis |
| Mesaj Kuyruğu | - |
| Background Jobs | Hangfire |
| Authentication | JWT Bearer |
| Password Hashing | BCrypt |
| Validation | FluentValidation |
| Mediator | MediatR 12 |

---

## 📁 Proje Yapısı
```
CardMerchantSystem/
├── src/
│   ├── CardMerchantSystem.API/
│   │   ├── Auth/
│   │   │   ├── Constants/
│   │   │   │   └── Permissions.cs
│   │   │   ├── Entities/
│   │   │   │   ├── UserEntity.cs
│   │   │   │   ├── RoleEntity.cs
│   │   │   │   └── UserRoleEntity.cs
│   │   │   ├── Enums/
│   │   │   │   └── SystemRole.cs
│   │   │   ├── Models/
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   ├── LoginResponse.cs
│   │   │   │   ├── RegisterRequest.cs
│   │   │   │   └── User.cs
│   │   │   ├── Persistence/
│   │   │   │   └── AuthDbContext.cs
│   │   │   └── Services/
│   │   │       ├── AuthService.cs
│   │   │       ├── IAuthService.cs
│   │   │       ├── IJwtService.cs
│   │   │       └── JwtService.cs
│   │   ├── Controllers/
│   │   │   ├── ApiControllerBase.cs
│   │   │   ├── AuthController.cs
│   │   │   └── ... (diğer controller'lar)
│   │   ├── Middleware/
│   │   │   ├── GlobalExceptionMiddleware.cs
│   │   │   └── GlobalExceptionMiddlewareExtensions.cs
│   │   ├── Models/
│   │   │   └── ApiErrorResponse.cs
│   │   └── Jobs/
│   │       ├── SettlementJob.cs
│   │       ├── DailyLimitResetJob.cs
│   │       └── MonthlyLimitResetJob.cs
│   ├── CardMerchantSystem.Shared/
│   │   ├── Abstractions/
│   │   └── Kernel/
│   │       ├── AggregateRoot.cs
│   │       ├── Entity.cs
│   │       ├── Enumeration.cs
│   │       ├── ErrorCodes.cs
│   │       ├── IDomainEvent.cs
│   │       ├── Result.cs
│   │       ├── ValueObject.cs
│   │       └── Exceptions/
│   │           ├── DomainException.cs
│   │           ├── NotFoundException.cs
│   │           ├── ValidationException.cs
│   │           ├── BusinessRuleException.cs
│   │           ├── ConflictException.cs
│   │           ├── UnauthorizedException.cs
│   │           └── ForbiddenException.cs
│   └── Modules/
│       ├── Card/
│       │   ├── Card.Domain/
│       │   ├── Card.Application/
│       │   └── Card.Infrastructure/
│       ├── Merchant/
│       ├── Transaction/
│       ├── Dispute/
│       ├── Campaign/
│       ├── BKM/
│       ├── HSM/
│       ├── Fee/
│       ├── Statement/
│       ├── Accounting/
│       ├── MerchantReport/
│       ├── MerchantSettlement/
│       ├── BulkCardPrint/
│       ├── RegulatoryReporting/
│       ├── Courier/
│       ├── EarlyBlockResolution/
│       └── WorkOrder/
└── tests/
```

---

## 📝 Önemli Notlar

### ApiControllerBase Kullanımı

Tüm controller'lar `ApiControllerBase`'den türemeli:
```csharp
[Authorize]
public class XxxController : ApiControllerBase
{
    // HandleNotFound(result, "Entity adı", id);
    // HandleResult(result);
    // CreatedResponse(nameof(GetById), new { id = value.Id }, value);
}
```

### Policy Kullanımı
```csharp
// Class seviyesinde
[Authorize(Policy = Policies.CardManagement)]
public class CardsController : ApiControllerBase

// Metod seviyesinde
[HttpDelete("{id:guid}")]
[Authorize(Policy = Policies.AdminOnly)]
public async Task<ActionResult> Delete(Guid id)
```

### Exception Fırlatma
```csharp
throw new NotFoundException("Kart", id);
throw new BusinessRuleException("Kart aktif değil");
throw new ConflictException("Bu kodla kayıt zaten mevcut");
throw new UnauthorizedException();
throw new ForbiddenException();
```

---

## 🔜 Sonraki Adımlar

1. **Validation Pipeline** - MediatR behavior ile otomatik validasyon
2. **Audit Trail** - Entity değişiklik takibi
3. **Health Checks** - API ve DB sağlık kontrolü
4. Kalan modüller (InstantCardPrint, Inventory)
5. Controller'lara Policy uygulama

---

## 📞 İletişim

Yeni chat açıldığında bu dosyayı paylaşarak devam edilebilir.