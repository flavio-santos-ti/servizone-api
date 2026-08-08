namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa a execução prática de um Ticket (Agregado separado).
/// No MVP: relacionamento 1:1 com Ticket.
/// Futura evolução: múltiplas visitas, atendimentos parciais.
/// </summary>
public class Attendance : TenantEntity
{
  /// <summary>
  /// Referência ao Ticket sendo executado.
  /// </summary>
  public Guid TicketId { get; private set; }

  /// <summary>
  /// Técnico executor do atendimento.
  /// </summary>
  public Guid TechnicianId { get; private set; }

  /// <summary>
  /// Equipe executora (opcional).
  /// </summary>
  public Guid? TeamId { get; private set; }

  /// <summary>
  /// Data/hora de início do atendimento.
  /// </summary>
  public DateTime? StartedAt { get; private set; }

  /// <summary>
  /// Data/hora de conclusão do atendimento.
  /// </summary>
  public DateTime? CompletedAt { get; private set; }

  /// <summary>
  /// Observações operacionais do atendimento.
  /// </summary>
  public string? Notes { get; private set; }

  /// <summary>
  /// Status do atendimento (in_progress, completed, cancelled).
  /// </summary>
  public string Status { get; private set; } = AttendanceStatus.InProgress;

  // Construtor privado para ORM
  private Attendance() : base()
  {
  }

  // Construtor para criação de novo Atendimento
  public Attendance(
    Guid id,
    Guid organizationId,
    Guid ticketId,
    Guid technicianId,
    Guid? teamId = null)
    : base(id, organizationId)
  {
    TicketId = ticketId;
    TechnicianId = technicianId;
    TeamId = teamId;
    Status = AttendanceStatus.InProgress;
  }

  /// <summary>
  /// Inicia o atendimento (registra StartedAt).
  /// </summary>
  public void Start()
  {
    if (StartedAt.HasValue)
      throw new InvalidOperationException("Atendimento já foi iniciado.");

    StartedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Completa o atendimento (registra CompletedAt).
  /// </summary>
  public void Complete(string? notes = null)
  {
    if (!StartedAt.HasValue)
      throw new InvalidOperationException("Atendimento não foi iniciado.");

    if (CompletedAt.HasValue)
      throw new InvalidOperationException("Atendimento já foi concluído.");

    CompletedAt = DateTime.UtcNow;
    Notes = notes;
    Status = AttendanceStatus.Completed;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Cancela o atendimento.
  /// </summary>
  public void Cancel(string? notes = null)
  {
    if (CompletedAt.HasValue)
      throw new InvalidOperationException("Não é possível cancelar um atendimento já concluído.");

    Notes = notes;
    Status = AttendanceStatus.Cancelled;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Adiciona observações ao atendimento.
  /// </summary>
  public void AddNotes(string notes)
  {
    if (string.IsNullOrWhiteSpace(notes))
      throw new ArgumentException("Observações não podem ser vazias.", nameof(notes));

    Notes = string.IsNullOrWhiteSpace(Notes) ? notes : $"{Notes}\n{notes}";
    UpdatedAt = DateTime.UtcNow;
  }
}

/// <summary>
/// Constantes de status do Atendimento.
/// </summary>
public static class AttendanceStatus
{
  public const string InProgress = "in_progress";
  public const string Completed = "completed";
  public const string Cancelled = "cancelled";
}
