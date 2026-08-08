using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Team.
/// </summary>
public interface ITeamRepository : IRepository<Team>
{
  /// <summary>
  /// Busca Equipes por status.
  /// </summary>
  Task<IEnumerable<Team>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Equipe por nome.
  /// </summary>
  Task<Team?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Equipes por especialidade.
  /// </summary>
  Task<IEnumerable<Team>> GetBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Técnicos membros de uma Equipe.
  /// </summary>
  Task<IEnumerable<Technician>> GetTeamMembersAsync(Guid teamId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Adiciona um Técnico a uma Equipe.
  /// </summary>
  Task AddTechnicianToTeamAsync(Guid teamId, Guid technicianId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Remove um Técnico de uma Equipe.
  /// </summary>
  Task RemoveTechnicianFromTeamAsync(Guid teamId, Guid technicianId, CancellationToken cancellationToken = default);
}
