using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade TicketHistory.
/// </summary>
public interface ITicketHistoryRepository : IRepository<TicketHistory>
{
  /// <summary>
  /// Busca histórico de um Ticket específico, ordenado por data.
  /// </summary>
  Task<IEnumerable<TicketHistory>> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca histórico de um Ticket por tipo de evento.
  /// </summary>
  Task<IEnumerable<TicketHistory>> GetByTicketAndEventTypeAsync(Guid ticketId, string eventType, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca histórico de ações de um usuário específico.
  /// </summary>
  Task<IEnumerable<TicketHistory>> GetByPerformedByAsync(string performedBy, CancellationToken cancellationToken = default);
}
