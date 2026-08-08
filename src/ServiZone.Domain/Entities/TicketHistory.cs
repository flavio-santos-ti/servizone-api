namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa um registro imutável de evento do ciclo de vida do Ticket.
/// Uma vez criado, nunca pode ser alterado ou deletado.
/// </summary>
public class TicketHistory : TenantEntity
{
  /// <summary>
  /// Referência ao Ticket.
  /// </summary>
  public Guid TicketId { get; private set; }

  /// <summary>
  /// Tipo do evento (status_changed, assigned, location_updated, note_added, etc.).
  /// </summary>
  public string EventType { get; private set; } = string.Empty;

  /// <summary>
  /// Valor anterior (JSONB).
  /// </summary>
  public string? OldValue { get; private set; }

  /// <summary>
  /// Novo valor (JSONB).
  /// </summary>
  public string? NewValue { get; private set; }

  /// <summary>
  /// Identificador de quem realizou a ação.
  /// </summary>
  public string PerformedBy { get; private set; } = string.Empty;

  /// <summary>
  /// Timestamp do evento.
  /// </summary>
  public DateTime PerformedAt { get; private set; }

  /// <summary>
  /// Observações adicionais.
  /// </summary>
  public string? Notes { get; private set; }

  // Construtor privado para ORM
  private TicketHistory() : base()
  {
  }

  // Construtor para criação de novo registro de histórico
  public TicketHistory(
    Guid id,
    Guid organizationId,
    Guid ticketId,
    string eventType,
    string performedBy,
    string? oldValue = null,
    string? newValue = null,
    string? notes = null,
    DateTime? performedAt = null)
    : base(id, organizationId)
  {
    if (string.IsNullOrWhiteSpace(eventType))
      throw new ArgumentException("Tipo do evento é obrigatório.", nameof(eventType));

    if (string.IsNullOrWhiteSpace(performedBy))
      throw new ArgumentException("Identificador de quem realizou a ação é obrigatório.", nameof(performedBy));

    TicketId = ticketId;
    EventType = eventType;
    PerformedBy = performedBy;
    OldValue = oldValue;
    NewValue = newValue;
    Notes = notes;
    PerformedAt = performedAt ?? DateTime.UtcNow;
  }

  // NENHUM MÉTODO DE MUTAÇÃO — Registro é imutável após criação
}

/// <summary>
/// Constantes de tipos de evento do histórico.
/// </summary>
public static class HistoryEventType
{
  public const string StatusChanged = "status_changed";
  public const string Assigned = "assigned";
  public const string LocationUpdated = "location_updated";
  public const string NoteAdded = "note_added";
  public const string PriorityChanged = "priority_changed";
  public const string DescriptionUpdated = "description_updated";
  public const string Cancelled = "cancelled";
  public const string Completed = "completed";
}
