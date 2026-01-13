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

## 🏗 Mimari

Proje **Clean Architecture** ve **Modüler Monolith** yaklaşımı ile tasarlanmıştır:
```
CardMerchantSystem/
├── src/
│   ├── CardMerchantSystem.API/          # REST API Layer
│   ├── CardMerchantSystem.Shared/       # Shared Kernel
│   └── Modules/
│       ├── Card/                        # Kart Modülü
│       │   ├── Card.Domain
│       │   ├── Card.Application
│       │   └── Card.Infrastructure
│       ├── Merchant/                    # Üye İşyeri Modülü
│       ├── Transaction/                 # İşlem Modülü
│       ├── Dispute/                     # İtiraz Modülü
│       ├── Campaign/                    # Kampanya Modülü
│       ├── BKM/                         # BKM Switch Modülü
│       ├── HSM/                         # HSM Modülü
│       ├── Fee/                         # Ücret Yönetimi Modülü
│       ├── Statement/                   # Ekstre Modülü
│       └── Accounting/                  # Muhasebe Modülü
└── tests/
    └── CardMerchantSystem.Tests/        # Unit & Integration Tests
```

### Katmanlar

| Katman | Sorumluluk |
|--------|------------|
| **Domain** | Entity, Value Object, Domain Event, Repository Interface |
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

## 🛠 Teknolojiler

### Backend
- **.NET 8** - Framework
- **ASP.NET Core Web API** - REST API
- **Entity Framework Core 8** - ORM
- **MediatR** - CQRS Pattern
- **FluentValidation** - Validation
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
```

4. **Uygulamayı çalıştırın**
```bash
cd src/CardMerchantSystem.API
dotnet run
```

5. **Swagger UI'a erişin**
```
https://localhost:7001/swagger
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

#### Komisyon Hesaplama
```http
POST /api/Fee/calculate-commission
Authorization: Bearer {token}
Content-Type: application/json

{
  "merchantId": "M001",
  "transactionAmount": 1000,
  "mcc": "5812",
  "installmentCount": 3
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
| BKM | BKMMessages, BINTables, ClearingRecords, SettlementRecords |
| HSM | HSMCommands, CryptoKeys |
| Fee | Tariffs, TariffRules, MerchantTariffs, FeeAccruals, MembershipFees, CommissionBreakdowns |
| Statement | CardStatements, StatementItems, StatementNotifications, StatementPeriodConfigs |
| Accounting | ChartOfAccounts, AccountingPeriods, JournalEntries, JournalEntryLines, AccountBalances |

### ER Diagram

Her modül kendi DbContext'ine sahiptir ve bağımsız olarak yönetilir.

## 🔒 Güvenlik

- **JWT Authentication**: Tüm API endpoint'leri JWT ile korunmaktadır
- **Password Hashing**: BCrypt algoritması kullanılmaktadır
- **HSM Integration**: Kritik kriptografik işlemler HSM üzerinde gerçekleştirilir
- **Input Validation**: FluentValidation ile tüm girdiler doğrulanır

## 📈 Planlanan Özellikler

- [ ] Üye İşyeri Ekstre/Raporlama
- [ ] Günsonu Muhasebe İşlemleri
- [ ] Toplu Kart Basım (Bileşim/Austuria)
- [ ] Yasal Raporlamalar (BDDK, TCMB, BKM)
- [ ] Kurye Entegrasyonu (Kuryenet)
- [ ] Erken Bloke Çözüm
- [ ] İş Emri Yönetimi
- [ ] Anında Kart Basım (Evolis)
- [ ] Envanter Yönetimi

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add some amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request açın

## 📄 Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 👨‍💻 Geliştirici

**Volkan** - .NET Developer

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!