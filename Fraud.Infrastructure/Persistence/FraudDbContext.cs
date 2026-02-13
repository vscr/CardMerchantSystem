using CardMerchantSystem.Shared.Extensions;
using Fraud.Domain.Entities;
using Fraud.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fraud.Infrastructure.Persistence;

public class FraudDbContext(DbContextOptions<FraudDbContext> options, IMediator? mediator) : DbContext(options)
{
    private readonly IMediator? _mediator = mediator;

    public DbSet<FraudRule> FraudRules => Set<FraudRule>();
    public DbSet<FraudRuleCondition> FraudRuleConditions => Set<FraudRuleCondition>();
    public DbSet<FraudScenario> FraudScenarios => Set<FraudScenario>();
    public DbSet<HitScenario> HitScenarios => Set<HitScenario>();
    public DbSet<FraudAlert> FraudAlerts => Set<FraudAlert>();
    public DbSet<FraudAction> FraudActions => Set<FraudAction>();
    public DbSet<CardFraudProfile> CardFraudProfiles => Set<CardFraudProfile>();
    public DbSet<FraudBlacklist> FraudBlacklists => Set<FraudBlacklist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("fraud");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FraudDbContext).Assembly);

        // Seed Data
        SeedRules(modelBuilder);
        SeedRuleConditions(modelBuilder);
        SeedScenarios(modelBuilder);
        SeedBlacklist(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        if (_mediator != null)
        {
            await _mediator.DispatchDomainEventsAsync(this);
        }

        return result;
    }

    // ═══════════════════════════════════════
    // SEED DATA
    // ═══════════════════════════════════════

    private static readonly DateTime SeedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // ── Rule ID'leri ──
    private static readonly Guid RuleHighAmount = Guid.Parse("A0000001-0000-0000-0000-000000000001");
    private static readonly Guid RuleForeignHigh = Guid.Parse("A0000001-0000-0000-0000-000000000002");
    private static readonly Guid RuleNightHigh = Guid.Parse("A0000001-0000-0000-0000-000000000003");
    private static readonly Guid RuleRiskyMcc = Guid.Parse("A0000001-0000-0000-0000-000000000004");
    private static readonly Guid RuleFrequent = Guid.Parse("A0000001-0000-0000-0000-000000000005");
    private static readonly Guid RuleDailyTotal = Guid.Parse("A0000001-0000-0000-0000-000000000006");
    private static readonly Guid RuleNoPinHigh = Guid.Parse("A0000001-0000-0000-0000-000000000007");
    private static readonly Guid FilterLowAmount = Guid.Parse("A0000001-0000-0000-0000-000000000008");

    private static void SeedRules(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudRule>().HasData(
            new
            {
                Id = RuleHighAmount,
                Code = "R001",
                Name = "Yüksek Tutarlı İşlem",
                Description = (string?)"Tek işlem tutarı 25.000 TL üzeri",
                RuleType = FraudRuleType.Simple,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = RuleForeignHigh,
                Code = "R002",
                Name = "Yurtdışı Yüksek Tutar",
                Description = (string?)"Yurtdışı işlem + 5.000 TL üzeri",
                RuleType = FraudRuleType.Complex,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = RuleNightHigh,
                Code = "R003",
                Name = "Gece Yüksek Tutar",
                Description = (string?)"02:00-06:00 arası + 10.000 TL üzeri",
                RuleType = FraudRuleType.Complex,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = RuleRiskyMcc,
                Code = "R004",
                Name = "Riskli MCC",
                Description = (string?)"Yüksek riskli üye işyeri kategori kodları",
                RuleType = FraudRuleType.Simple,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = RuleFrequent,
                Code = "R005",
                Name = "Saatlik Çok İşlem",
                Description = (string?)"Son 1 saatte aynı karttan 5+ işlem",
                RuleType = FraudRuleType.Periodic,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                PeriodMinutes = (int?)60,
                PeriodThreshold = (decimal?)5m,
                PeriodFunction = (string?)"COUNT",
                PeriodGroupBy = (string?)"CARD",
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = RuleDailyTotal,
                Code = "R006",
                Name = "Günlük Yüksek Toplam",
                Description = (string?)"Son 24 saatte 50.000 TL+ toplam işlem",
                RuleType = FraudRuleType.Periodic,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                PeriodMinutes = (int?)1440,
                PeriodThreshold = (decimal?)50000m,
                PeriodFunction = (string?)"SUM",
                PeriodGroupBy = (string?)"CARD",
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = RuleNoPinHigh,
                Code = "R007",
                Name = "PIN'siz Yüksek Tutar",
                Description = (string?)"PIN girilmemiş + 3.000 TL üzeri",
                RuleType = FraudRuleType.Complex,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = FilterLowAmount,
                Code = "F001",
                Name = "Düşük Tutar Filtresi",
                Description = (string?)"100 TL altı işlemleri hariç tut",
                RuleType = FraudRuleType.Simple,
                LogicalOperator = LogicalOperator.And,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            }
        );
    }

    private static void SeedRuleConditions(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudRuleCondition>().HasData(
            // R001: OriginalAmount > 25000
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000001"),
                FraudRuleId = RuleHighAmount,
                ParameterName = "OriginalAmount",
                Operator = RuleOperator.GreaterThan,
                Value = "25000",
                OrderIndex = 1,
                CreatedAt = SeedDate
            },
            // R002: MerchantCountryCode != TR
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000002"),
                FraudRuleId = RuleForeignHigh,
                ParameterName = "MerchantCountryCode",
                Operator = RuleOperator.NotEquals,
                Value = "TR",
                OrderIndex = 1,
                CreatedAt = SeedDate
            },
            // R002: OriginalAmount > 5000
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000003"),
                FraudRuleId = RuleForeignHigh,
                ParameterName = "OriginalAmount",
                Operator = RuleOperator.GreaterThan,
                Value = "5000",
                OrderIndex = 2,
                CreatedAt = SeedDate
            },
            // R003: TransactionHour >= 2
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000004"),
                FraudRuleId = RuleNightHigh,
                ParameterName = "TransactionHour",
                Operator = RuleOperator.GreaterThanOrEqual,
                Value = "2",
                OrderIndex = 1,
                CreatedAt = SeedDate
            },
            // R003: TransactionHour <= 6
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000005"),
                FraudRuleId = RuleNightHigh,
                ParameterName = "TransactionHour",
                Operator = RuleOperator.LessThanOrEqual,
                Value = "6",
                OrderIndex = 2,
                CreatedAt = SeedDate
            },
            // R003: OriginalAmount > 10000
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000006"),
                FraudRuleId = RuleNightHigh,
                ParameterName = "OriginalAmount",
                Operator = RuleOperator.GreaterThan,
                Value = "10000",
                OrderIndex = 3,
                CreatedAt = SeedDate
            },
            // R004: MCC In riskli kodlar
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000007"),
                FraudRuleId = RuleRiskyMcc,
                ParameterName = "Mcc",
                Operator = RuleOperator.In,
                Value = "6051,6211,7995,5967,5966",
                OrderIndex = 1,
                CreatedAt = SeedDate
            },
            // R007: IsPinEntered == false
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000008"),
                FraudRuleId = RuleNoPinHigh,
                ParameterName = "IsPinEntered",
                Operator = RuleOperator.Equals,
                Value = "False",
                OrderIndex = 1,
                CreatedAt = SeedDate
            },
            // R007: OriginalAmount > 3000
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000009"),
                FraudRuleId = RuleNoPinHigh,
                ParameterName = "OriginalAmount",
                Operator = RuleOperator.GreaterThan,
                Value = "3000",
                OrderIndex = 2,
                CreatedAt = SeedDate
            },
            // F001: OriginalAmount < 100
            new
            {
                Id = Guid.Parse("B0000001-0000-0000-0000-000000000010"),
                FraudRuleId = FilterLowAmount,
                ParameterName = "OriginalAmount",
                Operator = RuleOperator.LessThan,
                Value = "100",
                OrderIndex = 1,
                CreatedAt = SeedDate
            }
        );
    }

    private static void SeedScenarios(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudScenario>().HasData(
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000001"),
                ScenarioNo = 1,
                Name = "Yüksek Tutarlı İşlem Tespiti",
                Description = (string?)"25.000 TL üzeri tek işlem kontrolü",
                RuleId = RuleHighAmount,
                FilterRuleId = (Guid?)FilterLowAmount,
                CheckMode = FraudCheckMode.Online,
                FraudResponseCode = "05",
                Score = 70,
                RunOrder = 1,
                IsSimulation = false,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000002"),
                ScenarioNo = 2,
                Name = "Yurtdışı Şüpheli İşlem",
                Description = (string?)"Yurtdışı + yüksek tutar kombinasyonu",
                RuleId = RuleForeignHigh,
                CheckMode = FraudCheckMode.Online,
                FraudResponseCode = "01",
                Score = 85,
                RunOrder = 2,
                IsSimulation = false,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000003"),
                ScenarioNo = 3,
                Name = "Gece Saati Yüksek Tutar",
                Description = (string?)"02-06 arası yüksek tutarlı işlem",
                RuleId = RuleNightHigh,
                CheckMode = FraudCheckMode.Online,
                FraudResponseCode = "01",
                Score = 80,
                RunOrder = 3,
                IsSimulation = false,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000004"),
                ScenarioNo = 4,
                Name = "Riskli Kategori İşlemi",
                Description = (string?)"Yüksek riskli MCC kodlarında işlem",
                RuleId = RuleRiskyMcc,
                FilterRuleId = (Guid?)FilterLowAmount,
                CheckMode = FraudCheckMode.Both,
                FraudResponseCode = "01",
                Score = 60,
                RunOrder = 4,
                IsSimulation = true,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000005"),
                ScenarioNo = 5,
                Name = "Sıklık Anomalisi",
                Description = (string?)"Kısa sürede çok fazla işlem",
                RuleId = RuleFrequent,
                CheckMode = FraudCheckMode.Online,
                FraudResponseCode = "05",
                Score = 90,
                RunOrder = 5,
                IsSimulation = false,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000006"),
                ScenarioNo = 6,
                Name = "Günlük Limit Aşımı",
                Description = (string?)"24 saatte 50K+ toplam işlem",
                RuleId = RuleDailyTotal,
                CheckMode = FraudCheckMode.Offline,
                FraudResponseCode = "01",
                Score = 75,
                RunOrder = 6,
                IsSimulation = false,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("C0000001-0000-0000-0000-000000000007"),
                ScenarioNo = 7,
                Name = "PIN'siz Yüksek Tutar",
                Description = (string?)"PIN doğrulaması olmadan yüksek tutar",
                RuleId = RuleNoPinHigh,
                CheckMode = FraudCheckMode.Online,
                FraudResponseCode = "01",
                Score = 65,
                RunOrder = 7,
                IsSimulation = false,
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            }
        );
    }

    private static void SeedBlacklist(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FraudBlacklist>().HasData(
            new
            {
                Id = Guid.Parse("D0000001-0000-0000-0000-000000000001"),
                ListType = "Country",
                Value = "KP",
                IsBlacklist = true,
                Reason = (string?)"Kuzey Kore — yaptırım ülkesi",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("D0000001-0000-0000-0000-000000000002"),
                ListType = "Country",
                Value = "IR",
                IsBlacklist = true,
                Reason = (string?)"İran — yaptırım ülkesi",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("D0000001-0000-0000-0000-000000000003"),
                ListType = "Country",
                Value = "SY",
                IsBlacklist = true,
                Reason = (string?)"Suriye — yaptırım ülkesi",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("D0000001-0000-0000-0000-000000000004"),
                ListType = "MCC",
                Value = "7995",
                IsBlacklist = true,
                Reason = (string?)"Kumar — yüksek riskli MCC",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("D0000001-0000-0000-0000-000000000005"),
                ListType = "MCC",
                Value = "5967",
                IsBlacklist = true,
                Reason = (string?)"Doğrudan pazarlama — yüksek riskli",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            },
            new
            {
                Id = Guid.Parse("D0000001-0000-0000-0000-000000000006"),
                ListType = "Country",
                Value = "TR",
                IsBlacklist = false,
                Reason = (string?)"Türkiye — beyaz liste (yerli işlem)",
                IsActive = true,
                CreatedAt = SeedDate,
                CreatedBy = (string?)"system"
            }
        );
    }
}