using Courier.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Courier.Infrastructure.Persistence;

public class CourierDbContext : DbContext
{
    public CourierDbContext(DbContextOptions<CourierDbContext> options)
        : base(options)
    {
    }

    public DbSet<CourierCompany> CourierCompanies => Set<CourierCompany>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
    public DbSet<DeliveryAttempt> DeliveryAttempts => Set<DeliveryAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourierDbContext).Assembly);
    }
}