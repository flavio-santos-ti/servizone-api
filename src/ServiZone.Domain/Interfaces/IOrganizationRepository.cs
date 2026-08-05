using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Organization.
/// </summary>
public interface IOrganizationRepository : IRepository<Organization>
{
  /// <summary>
  /// Busca uma organização por seu slug único.
  /// </summary>
  Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

  /// <summary>
  /// Verifica se existe uma organização com o slug informado.
  /// </summary>
  Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
}
