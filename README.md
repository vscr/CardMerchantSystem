# 💳 Card Merchant System

Kurumsal düzeyde bir **Kart ve Üye İşyeri Yönetim Sistemi** - .NET 8, Clean Architecture ve Domain-Driven Design (DDD) prensipleri ile geliştirilmiştir.

![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)
![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)
![DDD](https://img.shields.io/badge/Pattern-Domain%20Driven%20Design-orange)

## 📋 İçindekiler

- [Proje Hakkında](#-proje-hakkında)
- [Mimari](#-mimari)
- [Modüller](#-modüller)
- [Teknolojiler](#-teknolojiler)
- [Kurulum](#-kurulum)
- [API Dokümantasyonu](#-api-dokümantasyonu)
- [Veritabanı Şeması](#-veritabanı-şeması)
- [Katkıda Bulunma](#-katkıda-bulunma)

## 🎯 Proje Hakkında

Card Merchant System, bankaların ve finansal kuruluşların kart operasyonlarını yönetmek için tasarlanmış kapsamlı bir backend sistemidir. Sistem şu temel işlevleri destekler:

- **Kart Yaşam Döngüsü Yönetimi**: Başvuru, onay, basım, teslimat
- **Üye İşyeri Yönetimi**: Kayıt, terminal atama, komisyon yönetimi
- **İşlem Yönetimi**: Satış, iade, provizyon, takas
- **Finansal Operasyonlar**: Ekstre, muhasebe, komisyon hesaplama
- **Güvenlik**: HSM entegrasyonu, PIN/CVV yönetimi
- **Entegrasyonlar**: BKM Switch, ISO 8583
- **Raporlama**: Yasal raporlar, üye işyeri raporları
- **Toplu İşlemler**: Toplu kart basım, batch işleme

## 🏗 Mimari

Proje **Clean Architecture** ve **Modüler Monolith** yaklaşımı ile tasarlanmıştır:
```
CardMerchantSystem/
├── src/
│   ├── API/
│   │   └── CardMerchantSystem.API/          # REST API Layer
│   ├── Shared/
│   │   └── CardMerchantSystem.Shared.Kernel # Shared Kernel (Entity, ValueObject, Result)
│   ├── Infrastructure/                       # Ortak altyapı bileşenleri
│   └── Modules/
│       ├── Accounting/                       # Muhasebe Modülü
│       │   ├── Accounting.Domain
│       │   ├── Accounting.Application
│       │   └── Accounting.Infrastructure
│       ├── BKM/                              # BKM Switch Modülü
│       ├── BulkCardPrint/                    # Toplu Kart Basım Modülü
│       ├── Campaign/                         # Kampanya Modülü
│       ├── Card/                             # Kart Modülü
│       ├── Dispute/                          # İtiraz Modülü
│       ├── Fee/                              # Ücret Yönetimi Modülü
│       ├── HSM/                              # HSM Modülü
│       ├── Merchant/                         # Üye İşyeri Modülü
│       ├── MerchantReport/                   # Üye İşyeri Raporlama Modülü
│       ├── MerchantSettlement/               # Üye İşyeri Takas Modülü
│       ├── RegulatoryReporting/              # Yasal Raporlama Modülü
│       ├── Statement/                        # Ekstre Modülü
│       └── Transaction/                      # İşlem Modülü
└── tests/
    └── CardMerchantSystem.Tests/             # Unit & Integration Tests
```

### Katmanlar

| Katman | Sorumluluk |
|--------|------------|
| **Domain** | Entity, Value Object, Domain Event, Repository Interface, Enumeration |
| **Application** | CQRS (Command/Query), DTO, Validator, Handler |
| **Infrastructure** | DbContext, Repository Implementation, External Services |
| **API** | Controller, Middleware, Authentication |

## 📦 Modüller

### 1. 💳 Card (Kart Yönetimi)
- Kart başvurusu ve onay süreçleri
- Kart durumu yönetimi (Aktif, Blokeli, İptal)
- Kart basım ve teslimat takibi
- Durum geçmişi

### 2. 🏪 Merchant (Üye İşyeri Yönetimi)
- Üye işyeri kaydı ve yönetimi
- Terminal atama ve yönetimi
- MCC (Merchant Category Code) yönetimi
- Aktivasyon/Deaktivasyon

### 3. 💰 Transaction (İşlem Yönetimi)
- Satış ve iade işlemleri
- LKS (Limit Kontrol Sistemi) - Redis
- Fraud detection
- Settlement (Takas)
- Provizyon yönetimi

### 4. 📝 Dispute (İtiraz Yönetimi)
- İtiraz yaşam döngüsü
- Belge yönetimi
- Not ekleme
- Chargeback süreçleri

### 5. 🎁 Campaign (Kampanya Yönetimi)
- İndirim kampanyaları
- Puan kampanyaları
- Kural motoru
- Bütçe takibi
- Kullanım limitleri

### 6. 🔄 BKM (BKM Switch Entegrasyonu)
- ISO 8583 mesaj işleme
- Authorization
- Clearing
- Settlement
- BIN tablosu yönetimi

### 7. 🔐 HSM (Hardware Security Module)
- Thales PayShield / Gemalto SafeNet desteği
- PIN işlemleri (Generate, Verify, Change, Translate)
- CVV işlemleri (Generate, Verify)
- Key yönetimi (ZMK, ZPK, TMK, TPK)
- Şifreleme/Deşifreleme
- MAC hesaplama

### 8. 💵 Fee (Ücret Yönetimi)
- Tarife yönetimi
- Komisyon hesaplama motoru
- MCC/Taksit/Hacim bazlı kurallar
- Aidat yönetimi
- Tahakkuk ve ödeme takibi

### 9. 📄 Statement (Ekstre Yönetimi)
- Ekstre oluşturma
- PDF üretimi (QuestPDF)
- Faiz hesaplama
- Minimum ödeme hesaplama
- E-posta/SMS bildirimi
- Ödeme takibi

### 10. 📊 Accounting (Muhasebe Yönetimi)
- Hesap planı
- Muhasebe fişi (Journal Entry)
- Çift taraflı kayıt sistemi
- Dönem yönetimi
- Mizan raporu
- Otomatik muhasebeleştirme

### 11. 📈 MerchantReport (Üye İşyeri Raporlama)
- Üye işyeri bazlı raporlar
- İşlem özeti raporları
- Komisyon raporları
- PDF/Excel çıktı

### 12. 🔄 MerchantSettlement (Üye İşyeri Takas)
- Günsonu kapama
- Takas hesaplama
- Hakediş hesaplama (komisyon kesintisi)
- Ödeme planı
- Banka mutabakatı
- Settlement raporları

### 13. 🖨️ BulkCardPrint (Toplu Kart Basım)
- Basım batch'i oluşturma
- Kart üreticisine dosya üretimi
- Vendor entegrasyonu (FTP/API)
- Basım durumu takibi
- Kalite kontrol süreçleri

### 14. 📋 RegulatoryReporting (Yasal Raporlama)
- BDDK raporları
- TCMB raporları
- SPK raporları
- MASAK raporları
- BKM raporları
- Otomatik rapor üretimi ve zamanlama
- Rapor gönderim takibi

## 🛠 Teknolojiler

### Backend
- **.NET 8** - Framework
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 8** - ORM
- **MediatR 12.2** - CQRS Pattern
- **FluentValidation 11.9** - Validation
- **Hangfire** - Background Jobs

### Veritabanı & Cache
- **SQL Server** - Ana veritabanı
- **Redis** - LKS Cache

### Güvenlik
- **JWT Bearer** - Authentication
- **BCrypt** - Password Hashing
- **HSM Integration** - Cryptographic Operations

### Dokümantasyon & Test
- **Swagger/OpenAPI** - API Documentation
- **xUnit** - Unit Testing
- **Moq** - Mocking

### PDF & Raporlama
- **QuestPDF** - PDF Generation

## 🚀 Kurulum

### Gereksinimler

- .NET 8 SDK
- SQL Server 2019+
- Redis (opsiyonel - LKS için)
- Visual Studio 2022 veya VS Code

### Adımlar

1. **Repository'yi klonlayın**
```bash
git clone https://github.com/yourusername/CardMerchantSystem.git
cd CardMerchantSystem
```

2. **Veritabanı bağlantı ayarlarını yapın**

`appsettings.json` dosyasını düzenleyin:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=CardMerchantDb;Trusted_Connection=True;TrustServerCertificate=True;",
    "Redis": "localhost:6379"
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyHere123456789012",
    "Issuer": "CardMerchantSystem",
    "Audience": "CardMerchantSystem"
  }
}
```

3. **Migration'ları çalıştırın**
```bash
# Her modül için migration
dotnet ef database update --context CardDbContext
dotnet ef database update --context MerchantDbContext
dotnet ef database update --context TransactionDbContext
dotnet ef database update --context DisputeDbContext
dotnet ef database update --context CampaignDbContext
dotnet ef database update --context BKMDbContext
dotnet ef database update --context HSMDbContext
dotnet ef database update --context FeeDbContext
dotnet ef database update --context StatementDbContext
dotnet ef database update --context AccountingDbContext
dotnet ef database update --context MerchantReportDbContext
dotnet ef database update --context MerchantSettlementDbContext
dotnet ef database update --context BulkCardPrintDbContext
dotnet ef database update --context RegulatoryReportingDbContext
```

4. **Uygulamayı çalıştırın**
```bash
cd src/API/CardMerchantSystem.API
dotnet run
```

5. **Swagger UI'a erişin**
```
https://localhost:7202/swagger
```

## 📚 API Dokümantasyonu

### Authentication
```http
POST /api/Auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

Response:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiration": "2025-01-15T12:00:00Z",
  "username": "admin"
}
```

### Örnek API Çağrıları

#### Kart Başvurusu
```http
POST /api/CardApplications
Authorization: Bearer {token}
Content-Type: application/json

{
  "customerName": "Ahmet Yılmaz",
  "customerIdentityNumber": "12345678901",
  "customerEmail": "ahmet@example.com",
  "customerPhone": "5551234567",
  "cardTypeId": 1,
  "requestedLimit": 10000
}
```

#### İşlem Oluşturma
```http
POST /api/Transactions
Authorization: Bearer {token}
Content-Type: application/json

{
  "cardNumber": "4539980000000001",
  "merchantId": "M001",
  "terminalId": "T001",
  "amount": 150.50,
  "transactionTypeId": 1,
  "installmentCount": 1
}
```

#### Yasal Rapor Üretimi
```http
POST /api/GeneratedReports/generate
Authorization: Bearer {token}
Content-Type: application/json

{
  "reportDefinitionId": "guid",
  "periodStart": "2025-01-01",
  "periodEnd": "2025-01-31"
}
```

#### Toplu Kart Basım Batch'i Oluşturma
```http
POST /api/PrintBatches
Authorization: Bearer {token}
Content-Type: application/json

{
  "printVendorId": "guid",
  "cardApplicationIds": ["guid1", "guid2", "guid3"]
}
```

## 🗄 Veritabanı Şeması

### Ana Tablolar

| Modül | Tablolar |
|-------|----------|
| Card | Cards, CardApplications, CardStatusHistories, CardPrintQueues, CardDeliveries |
| Merchant | Merchants, Terminals |
| Transaction | Transactions, Settlements |
| Dispute | Disputes, DisputeDocuments, DisputeNotes |
| Campaign | Campaigns, CampaignRules, CampaignUsages |
| BKM | BKMMessages, BINTables, ClearingRecords, SettlementBatches, BankSettlementSummaries |
| HSM | HSMCommands, CryptoKeys |
| Fee | Tariffs, TariffRules, MerchantTariffs, FeeAccruals, MembershipFees, CommissionBreakdowns |
| Statement | CardStatements, StatementItems, StatementNotifications, StatementPeriodConfigs |
| Accounting | ChartOfAccounts, AccountingPeriods, JournalEntries, JournalEntryLines, AccountBalances |
| MerchantReport | MerchantReports, MerchantReportItems |
| MerchantSettlement | MerchantSettlementBatches, MerchantSettlementDetails, MerchantPayouts, MerchantReconciliations, MerchantReconciliationMismatches, DailySettlementSummaries |
| BulkCardPrint | PrintVendors, PrintBatches, PrintBatchItems |
| RegulatoryReporting | ReportDefinitions, ReportSchedules, GeneratedReports, ReportSubmissions |

### ER Diagram

Her modül kendi DbContext'ine sahiptir ve bağımsız olarak yönetilir. Toplam **14 ayrı DbContext** mevcuttur.

## 🔒 Güvenlik

- **JWT Authentication**: Tüm API endpoint'leri JWT ile korunmaktadır
- **Password Hashing**: BCrypt algoritması kullanılmaktadır
- **HSM Integration**: Kritik kriptografik işlemler HSM üzerinde gerçekleştirilir
- **Input Validation**: FluentValidation ile tüm girdiler doğrulanır

## ✅ Tamamlanan Modüller

- [x] Kart Yönetimi (Card)
- [x] Üye İşyeri Yönetimi (Merchant)
- [x] İşlem Yönetimi (Transaction)
- [x] İtiraz Yönetimi (Dispute)
- [x] Kampanya Yönetimi (Campaign)
- [x] BKM Switch Entegrasyonu (BKM)
- [x] HSM Entegrasyonu (HSM)
- [x] Ücret Yönetimi (Fee)
- [x] Ekstre Yönetimi (Statement)
- [x] Muhasebe Yönetimi (Accounting)
- [x] Üye İşyeri Raporlama (MerchantReport)
- [x] Üye İşyeri Takas (MerchantSettlement)
- [x] Toplu Kart Basım (BulkCardPrint)
- [x] Yasal Raporlama (RegulatoryReporting)

## 📈 Planlanan Özellikler

- [ ] Kurye Entegrasyonu (Kuryenet)
- [ ] Erken Bloke Çözüm
- [ ] İş Emri Yönetimi
- [ ] Anında Kart Basım (Evolis)
- [ ] Envanter Yönetimi
- [ ] React Frontend Paneli

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add some amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 👨‍💻 Geliştirici

**Volkan** - Senior .NET Developer

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!

## 📊 Proje İstatistikleri

| Metrik | Değer |
|--------|-------|
| Toplam Proje | 45 |
| Modül Sayısı | 14 |
| DbContext Sayısı | 14 |
| API Controller Sayısı | 25+ |
| Entity Sayısı | 60+ |