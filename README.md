# Card Merchant System

Bankacılık sektörü için **production-ready** Kart ve Üye İşyeri Yönetim Sistemi.

---

## Proje Özeti

| | |
|---|---|
| **Tamamlanma** | %97 (18/19 modül) |
| **Mimari** | Clean Architecture + DDD + CQRS + Event-Driven |
| **Veritabanı** | SQL Server + PostgreSQL (Multi-DB) |
| **Cache** | Redis (LKS, Fraud, Idempotency) |
| **Resilience** | Polly + Rate Limiting |
| **Performance** | Dapper + Snapshot Isolation — 150 TPS load tested |
| **Monitoring** | Serilog + Elasticsearch + Kibana |
| **Auth** | Dual-mode (Keycloak RS256 / Local JWT HS256), 7 rol |
| **Versiyon** | 1.8.0 |

---

## Modüller

### Tamamlanan (18)

| Modül | Açıklama | Öne Çıkan |
|-------|----------|-----------|
| **Card** | Başvuru → Basım → Teslimat lifecycle | Domain Events, State Machine |
| **Merchant** | Üye işyeri, terminal yönetimi | Multi-DB (MSSQL + PG), CQRS |
| **Transaction** | İşlem, provizyon, LKS | Polly, Redis, Rate Limiting, **Idempotency**, Dapper |
| **Fraud** | Senaryo tabanlı fraud detection engine | PayGuard mimarisi, real-time + offline, kural motoru |
| **Dispute** | İtiraz yönetimi | FraudDetected event dinler |
| **Campaign** | Kampanya, kural motoru | Post-transaction benefit hesaplama |
| **BKM** | Switch entegrasyonu | TerminalActivated event dinler |
| **HSM** | Güvenlik modülü (Thales, Gemalto) | Key hierarchy, PIN/CVV |
| **Fee** | Ücret, aidat, tarife | TransactionCompleted event dinler |
| **Statement** | Ekstre yönetimi | Document generation |
| **Accounting** | Muhasebe entegrasyonu | TransactionCompleted event dinler |
| **MerchantReport** | Üye işyeri raporlama | FTP, mail dağıtım |
| **MerchantSettlement** | Üye işyeri takas | Payment processing |
| **BulkCardPrint** | Toplu kart basım (Bileşim, Austria) | CardApproved event dinler |
| **RegulatoryReporting** | Yasal raporlama (BDDK, TCMB) | Compliance |
| **Courier** | Kurye entegrasyonu (Kuryenet) | Shipment tracking |
| **EarlyBlockResolution** | Erken bloke çözüm | — |
| **WorkOrder** | İş emri yönetimi | Beko, Ingenico, Teknoser |

### Planlanmış (1)

| Modül | Açıklama |
|-------|----------|
| **InstantCardPrint** | Evolis Primacy şube içi basım |

---

## Fraud Detection Engine

PayGuard production sistemi referans alınarak geliştirilen senaryo tabanlı fraud engine.

### Akış

```
Transaction → FraudEngine.CheckOnlineAsync()
  → Aktif senaryoları yükle (RunOrder sırasıyla)
  → Her senaryo için:
     Filter Rule → (true ise atla)
     Main Rule → (true ise HIT)
       → HitScenario kaydet
       → FraudDetectedEvent fırlat
       → CardFraudProfile güncelle
  → En yüksek skorlu sonuç: Score ≥ 80 → Reject, < 80 → Review
```

### Kural Tipleri

| Tip | Açıklama | Örnek |
|-----|----------|-------|
| **Simple** | Tek koşul | Amount > 25.000 |
| **Complex** | AND/OR koşul grubu | Yurtdışı + 5.000 TL üzeri |
| **Periodic** | Zaman bazlı istatistik | 1 saatte 5+ işlem |
| **Linked** | SQL script tabanlı | Cross-table analiz |

### Seed Senaryolar

| Senaryo | Kural | Score | Mod |
|---------|-------|-------|-----|
| Yüksek Tutar | > 25K TL | 70 | Online |
| Yurtdışı Şüpheli | Country ≠ TR + > 5K | 85 | Online |
| Gece Yüksek Tutar | 02-06 arası + > 10K | 80 | Online |
| Sıklık Anomalisi | 1h'de 5+ işlem | 90 | Online |
| Günlük Limit Aşımı | 24h'de > 50K toplam | 75 | Offline |
| PIN'siz Yüksek | PIN yok + > 3K | 65 | Online |
| Riskli MCC | Kumar, doğrudan pazarlama | 60 | Both (Simülasyon) |

---

## Idempotency

Bankacılık standardına uygun, Redis + DB hybrid idempotency mekanizması.

### Neden Gerekli

POS terminalleri ağ kesintisinde aynı işlemi tekrar gönderir. Idempotency key ile ilk sonuç cache'den döner, mükerrer işlem engellenir.

### Mekanizma

```
Client → POST /api/Transactions/process
         Header: Idempotency-Key: {terminal}:{rrn}

1. Redis GET → key var mı?
2. Yoksa → Redis SET NX (atomic lock)
3. İşlem çalışır → sonuç Redis + DB'ye yazılır (24h TTL)
4. Aynı key tekrar → cache'den döner
5. Aynı key + farklı body → 409 Conflict (SHA256 hash kontrolü)
```

| Durum | Davranış |
|-------|----------|
| İlk istek | Normal işlem, sonuç cache'lenir |
| Aynı key + aynı body | Cache'den döner (işlem yapılmaz) |
| Aynı key + farklı body | 409 Conflict |
| Processing sırasında retry | "İşlem işleniyor" hatası |
| 5xx hata | Lock serbest bırakılır (retry yapılabilsin) |
| 4xx hata | Hata cache'lenir (aynı hatalı veri → aynı hata) |

---

## Limit Kontrol Sistemi (LKS)

Dinamik, panelden yönetilebilir limit yapısı. Redis cache + DB hybrid.

### Çözümleme Önceliği

```
1. CardApplication (kart bazlı — Card modülünden, ICardLimitProvider ile)
2. CardLimitDefinition — CARD type (override)
3. CardLimitDefinition — BIN type
4. CardLimitDefinition — DEFAULT type (panelden yönetilen)
5. Fallback hardcoded (10.000 / 50.000)
```

### API Endpoints

| Method | Endpoint | Açıklama |
|--------|----------|----------|
| GET | `/api/CardLimits` | Tüm limit tanımları |
| GET | `/api/CardLimits/card/{cardNo}/usage` | Kart kullanım durumu |
| POST | `/api/CardLimits` | Yeni limit tanımı |
| PUT | `/api/CardLimits/{id}` | Limit güncelle |
| PATCH | `/api/CardLimits/{id}/toggle` | Aktif/pasif |
| POST | `/api/CardLimits/invalidate-cache` | Cache temizle |

---

## İşlem Akışı (Uçtan Uca)

```
POS/Client
    │  Idempotency-Key: T1017506:260214001
    ▼
ProcessTransactionCommand
    │
    ├─ 1. IdempotencyBehavior → key kontrol (Redis SET NX)
    ├─ 2. Validation → TransactionType, Amount
    ├─ 3. FraudEngine → Senaryo tarama (< 100ms hedef)
    │     ├─ Clean (score < 80) → devam
    │     ├─ Suspicious → devam (Review)
    │     └─ Fraudulent (score ≥ 80) → REJECT "05"
    ├─ 4. LimitService → Limit kontrol (Redis + DB)
    │     ├─ Tek işlem limiti
    │     ├─ Günlük limit
    │     └─ Aylık limit → aşarsa REJECT "51"
    ├─ 5. Limit Reserve → Redis INCR
    ├─ 6. Approve → AuthorizationCode üret
    ├─ 7. Limit Commit
    ├─ 8. Save → DB + Domain Events
    │     ├─ TransactionCompletedIntegrationEvent
    │     │     ├─ → Campaign (kazanım hesapla)
    │     │     ├─ → Fee (komisyon hesapla)
    │     │     └─ → Accounting (muhasebe kaydı)
    │     └─ FraudDetectedIntegrationEvent (fraud hit varsa)
    │           └─ → Dispute (itiraz kaydı)
    └─ 9. Idempotency Complete → sonuç cache'le (24h)
```

---

## Event-Driven Mimari

### Integration Events

| Event | Tetikleyen | Dinleyen |
|-------|-----------|----------|
| `CardApplicationApprovedIntegrationEvent` | Card.Approve() | BulkCardPrint, HSM |
| `TerminalActivatedIntegrationEvent` | Merchant.ActivateTerminal() | HSM, BKM |
| `TransactionCompletedIntegrationEvent` | Transaction.Approve() | Campaign, Fee, Accounting |
| `FraudDetectedIntegrationEvent` | Transaction.SetFraudCheckResult() | Dispute |
| `CardBlockedByFraudEvent` | FraudAlert.Resolve() | Card |

---

## Mimari

```
┌─────────────────────────────────────────────────────────┐
│  API Layer — Controllers, JWT Auth, Middleware           │
│  IdempotencyMiddleware, CorrelationId, RateLimiting     │
├─────────────────────────────────────────────────────────┤
│  Application Layer — Commands/Queries (MediatR)         │
│  IdempotencyBehavior, Event Handlers, Services          │
├─────────────────────────────────────────────────────────┤
│  Domain Layer — Aggregates, Value Objects, Smart Enums  │
│  Domain Events, Integration Events, Business Rules      │
├─────────────────────────────────────────────────────────┤
│  Infrastructure Layer — EF Core (Write), Dapper (Read)  │
│  Redis, Polly, IdempotencyStore, FraudEngine            │
├─────────────────────────────────────────────────────────┤
│  Shared Layer — Audit Trail, Kernel, Idempotency        │
│  ICardLimitProvider, Events, Data Extensions            │
└─────────────────────────────────────────────────────────┘
```

### Kullanılan Pattern'ler

| Pattern | Açıklama |
|---------|----------|
| Clean Architecture | Katmanlı mimari, dependency inversion |
| DDD | Aggregates, Value Objects, Smart Enums |
| CQRS | EF Core (Write) + Dapper (Read) |
| Event-Driven | MediatR domain + integration events |
| Repository | Data access abstraction |
| Idempotency | Redis SET NX + DB hybrid |
| Strategy | Fraud rule evaluator |
| State Machine | Card application lifecycle |
| Interceptor | Audit trail (EF Core SaveChanges) |

---

## Teknolojiler

| Kategori | Teknoloji |
|----------|-----------|
| Framework | .NET 8 |
| ORM | Entity Framework Core 8 (Write), Dapper (Read) |
| Database | SQL Server + PostgreSQL |
| Cache | Redis |
| Mediator | MediatR 12 |
| Resilience | Polly 8.x |
| Auth | JWT + BCrypt + Keycloak |
| Jobs | Hangfire |
| Logging | Serilog + Elasticsearch + Kibana |
| Container | Docker Compose |

---

## Hızlı Başlangıç

### 1. Docker Servisleri

```bash
docker-compose up -d
```

PostgreSQL (5432), Redis (6379), Elasticsearch (9200), Kibana (5601)

### 2. Migration

```powershell
Update-Database -Context AuthDbContext
Update-Database -Context AuditDbContext
Update-Database -Context CardDbContext
Update-Database -Context MerchantDbContext
Update-Database -Context TransactionDbContext
Update-Database -Context FraudDbContext
Update-Database -Context CampaignDbContext
# ... diğer modüller
```

### 3. Çalıştır

```bash
dotnet run --project CardMerchantSystem.API
```

Swagger: `https://localhost:7202/swagger`

### 4. Test Kullanıcıları

| Username | Password | Rol |
|----------|----------|-----|
| admin | Admin123! | Admin |
| callcenter | Test123! | CallCenterAgent |

---

## Güvenlik & Roller

| Rol | Erişim |
|-----|--------|
| Admin | Tüm yetkiler |
| CardOperator | Kart operasyonları, fraud alert |
| MerchantOperator | Üye işyeri operasyonları |
| FinanceOperator | Finans işlemleri |
| ComplianceOfficer | Yasal raporlama, audit, fraud alert |
| CallCenterAgent | Çağrı merkezi |
| Viewer | Sadece görüntüleme |

---

**Son Güncelleme:** 18 Şubat 2026
**Versiyon:** 1.8.0