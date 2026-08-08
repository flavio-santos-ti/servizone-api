using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Technician.
/// </summary>
public interface ITechnicianRepository : IRepository<Technician>
{
  /// <summary>
  /// Busca Técnicos por status.
  /// </summary>
  Task<IEnumerable<Technician>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Técnico por e-mail.
  /// </summary>
  Task<Technician?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Técnicos disponíveis (active) em uma área geográfica.
  /// </summary>
  Task<IEnumerable<Technician>> GetAvailableInAreaAsync(double latitude, double longitude, double radiusKm, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Técnicos por especialidade.
  /// </summary>
  Task<IEnumerable<Technician>> GetBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default);
}
