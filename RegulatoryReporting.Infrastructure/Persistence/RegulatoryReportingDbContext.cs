using Microsoft.EntityFrameworkCore;
using RegulatoryReporting.Domain.Entities;

namespace RegulatoryReporting.Infrastructure.Persistence;

public class RegulatoryReportingDbContext : DbContext
{
    public RegulatoryReportingDbContext(DbContextOptions<RegulatoryReportingDbContext> options)
        : base(options)
    {
    }

    public DbSet<ReportDefinition> ReportDefinitions => Set<ReportDefinition>();
    public DbSet<ReportSchedule> ReportSchedules => Set<ReportSchedule>();
    public DbSet<GeneratedReport> GeneratedReports => Set<GeneratedReport>();
    public DbSet<ReportSubmission> ReportSubmissions => Set<ReportSubmission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RegulatoryReportingDbContext).Assembly);
    }
}