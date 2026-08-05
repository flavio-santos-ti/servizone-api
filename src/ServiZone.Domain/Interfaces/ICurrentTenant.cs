namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Interface para resolução do tenant (organização) atual da requisição.
/// O OrganizationId é extraído do JWT pelo TenantMiddleware.
/// </summary>
public interface ICurrentTenant
{
  /// <summary>
  /// Identificador da Organização do contexto atual.
  /// </summary>
  Guid OrganizationId { get; }

  /// <summary>
  /// Indica se o tenant foi resolvido com sucesso.
  /// </summary>
  bool IsAvailable { get; }
}
