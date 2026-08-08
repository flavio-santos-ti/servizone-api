using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Attendance.
/// </summary>
public interface IAttendanceRepository : IRepository<Attendance>
{
  /// <summary>
  /// Busca Atendimento por Ticket.
  /// No MVP: relacionamento 1:1 com Ticket.
  /// </summary>
  Task<Attendance?> GetByTicketAsync(Guid ticketId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Atendimentos de um Técnico específico.
  /// </summary>
  Task<IEnumerable<Attendance>> GetByTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Atendimentos de uma Equipe específica.
  /// </summary>
  Task<IEnumerable<Attendance>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Atendimentos por status.
  /// </summary>
  Task<IEnumerable<Attendance>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Atendimentos em andamento de um Técnico.
  /// </summary>
  Task<IEnumerable<Attendance>> GetInProgressByTechnicianAsync(Guid technicianId, CancellationToken cancellationToken = default);
}
