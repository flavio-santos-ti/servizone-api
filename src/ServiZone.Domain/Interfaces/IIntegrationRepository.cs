using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Integration.
/// </summary>
public interface IIntegrationRepository : IRepository<Integration>
{
  /// <summary>
  /// Busca Integração por hash de API Key.
  /// </summary>
  Task<Integration?> GetByApiKeyHashAsync(string apiKeyHash, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Integrações por tipo de sistema.
  /// </summary>
  Task<IEnumerable<Integration>> GetBySystemTypeAsync(string systemType, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Integrações por status.
  /// </summary>
  Task<IEnumerable<Integration>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Integração ativa por API Key (valida hash).
  /// </summary>
  Task<Integration?> GetActiveByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
}
