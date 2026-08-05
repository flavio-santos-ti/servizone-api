namespace ServiZone.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades multi-tenant.
/// Toda entidade multi-tenant está vinculada a uma Organização (tenant).
/// </summary>
public abstract class TenantEntity : Entity
{
  /// <summary>
  /// Identificador da Organização à qual esta entidade pertence.
  /// O isolamento multi-tenant é aplicado via Global Query Filter do EF Core.
  /// </summary>
  public Guid OrganizationId { get; protected set; }

  protected TenantEntity() : base()
  {
  }

  protected TenantEntity(Guid organizationId) : base()
  {
    OrganizationId = organizationId;
  }

  protected TenantEntity(Guid id, Guid organizationId) : base(id)
  {
    OrganizationId = organizationId;
  }
}
