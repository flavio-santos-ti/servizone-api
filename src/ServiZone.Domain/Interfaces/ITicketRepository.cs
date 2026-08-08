using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Ticket.
/// </summary>
public interface ITicketRepository : IRepository<Ticket>
{
  /// <summary>
  /// Busca Tickets por status.
  /// </summary>
  Task<IEnumerable<Ticket>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Tickets atribuídos a um Técnico específico.
  /// </summary>
  Task<IEnumerable<Ticket>> GetByTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Tickets de uma Equipe específica.
  /// </summary>
  Task<IEnumerable<Ticket>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Tickets de um Cliente específico.
  /// </summary>
  Task<IEnumerable<Ticket>> GetByClientAsync(Guid clientId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Tickets de uma Integração específica.
  /// </summary>
  Task<IEnumerable<Ticket>> GetByIntegrationAsync(Guid integrationId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Ticket por identificador externo.
  /// </summary>
  Task<Ticket?> GetByExternalIdAsync(string systemType, string externalValue, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Tickets aguardando distribuição.
  /// </summary>
  Task<IEnumerable<Ticket>> GetPendingDistributionAsync(CancellationToken cancellationToken = default);
}
