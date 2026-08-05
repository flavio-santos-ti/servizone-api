using Microsoft.EntityFrameworkCore;
using ServiZone.Domain.Entities;
using ServiZone.Domain.Interfaces;

namespace ServiZone.Infrastructure.Data;

/// <summary>
/// Contexto do banco de dados da aplicação ServiZone.
/// Implementa multi-tenancy via Global Query Filter aplicado em todas as entidades TenantEntity.
/// </summary>
public class ServiZoneDbContext : DbContext
{
  private readonly ICurrentTenant _currentTenant;

  public ServiZoneDbContext(DbContextOptions<ServiZoneDbContext> options, ICurrentTenant currentTenant)
      : base(options)
  {
    _currentTenant = currentTenant;
  }

  // DbSets
  public DbSet<Organization> Organizations => Set<Organization>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // Aplicar configurações de entidades
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServiZoneDbContext).Assembly);

    // Aplicar Global Query Filter para multi-tenancy
    // Todas as entidades que herdam de TenantEntity são filtradas automaticamente por OrganizationId
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
      if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
      {
        var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
        var property = System.Linq.Expressions.Expression.Property(parameter, nameof(TenantEntity.OrganizationId));
        var organizationId = System.Linq.Expressions.Expression.Property(
            System.Linq.Expressions.Expression.Constant(_currentTenant),
            nameof(ICurrentTenant.OrganizationId));
        var body = System.Linq.Expressions.Expression.Equal(property, organizationId);
        var lambda = System.Linq.Expressions.Expression.Lambda(body, parameter);

        modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
      }
    }
  }

  public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    // Atualizar UpdatedAt automaticamente
    foreach (var entry in ChangeTracker.Entries<Entity>())
    {
      if (entry.State == EntityState.Modified)
      {
        entry.Property(nameof(Entity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
      }
    }

    return base.SaveChangesAsync(cancellationToken);
  }
}
