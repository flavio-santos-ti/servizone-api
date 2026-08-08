using ServiZone.Domain.ValueObjects;

namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa um Ticket — unidade de trabalho operacional.
/// Controla seu ciclo de vida através de máquina de estados.
/// Status só pode ser alterado via métodos de domínio, nunca diretamente.
/// </summary>
public class Ticket : TenantEntity
{
  /// <summary>
  /// Assunto do Ticket.
  /// </summary>
  public string Subject { get; private set; } = string.Empty;

  /// <summary>
  /// Descrição detalhada do Ticket.
  /// </summary>
  public string? Description { get; private set; }

  /// <summary>
  /// Status atual do Ticket.
  /// Só pode ser alterado via métodos de domínio (Open, Accept, Complete, etc.).
  /// </summary>
  public string Status { get; private set; } = TicketStatus.Recebido;

  /// <summary>
  /// Prioridade do Ticket.
  /// </summary>
  public Priority Priority { get; private set; } = ValueObjects.Priority.Normal;

  /// <summary>
  /// Tipo de serviço do Ticket.
  /// </summary>
  public ServiceType ServiceType { get; private set; } = new ServiceType("default");

  /// <summary>
  /// Endereço congelado do Local de Atendimento.
  /// Não muda mesmo se o cadastro do Cliente for alterado.
  /// </summary>
  public ServiceAddress ServiceAddress { get; private set; } = null!;

  /// <summary>
  /// Referência ao Cliente (destinatário do serviço).
  /// </summary>
  public Guid? ClientId { get; private set; }

  /// <summary>
  /// Referência à Integração de origem.
  /// </summary>
  public Guid? IntegrationId { get; private set; }

  /// <summary>
  /// Identificador no sistema externo.
  /// </summary>
  public ExternalId? ExternalId { get; private set; }

  /// <summary>
  /// Técnico atribuído após Aceite.
  /// Preenchido somente após o Técnico aceitar o Ticket.
  /// </summary>
  public Guid? AssignedTechnicianId { get; private set; }

  /// <summary>
  /// Equipe atribuída (opcional).
  /// </summary>
  public Guid? AssignedTeamId { get; private set; }

  /// <summary>
  /// Data/hora de conclusão do Ticket.
  /// </summary>
  public DateTime? CompletedAt { get; private set; }

  /// <summary>
  /// Data/hora de cancelamento do Ticket.
  /// </summary>
  public DateTime? CancelledAt { get; private set; }

  /// <summary>
  /// Motivo de recusa ou cancelamento.
  /// </summary>
  public string? RefusalReason { get; private set; }

  // Construtor privado para ORM
  private Ticket() : base()
  {
  }

  // Construtor para criação de novo Ticket
  public Ticket(
    Guid id,
    Guid organizationId,
    string subject,
    ServiceAddress serviceAddress,
    Priority priority,
    ServiceType serviceType,
    string? description = null,
    Guid? clientId = null,
    Guid? integrationId = null,
    ExternalId? externalId = null)
    : base(id, organizationId)
  {
    if (string.IsNullOrWhiteSpace(subject))
      throw new ArgumentException("Assunto do ticket é obrigatório.", nameof(subject));

    Subject = subject;
    Description = description;
    ServiceAddress = serviceAddress ?? throw new ArgumentNullException(nameof(serviceAddress));
    Priority = priority ?? throw new ArgumentNullException(nameof(priority));
    ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
    ClientId = clientId;
    IntegrationId = integrationId;
    ExternalId = externalId;
    Status = TicketStatus.Recebido;
  }

  // ========================================
  // Métodos de transição de status
  // ========================================

  /// <summary>
  /// Transição: Recebido → Aberto.
  /// </summary>
  public void Open()
  {
    if (Status != TicketStatus.Recebido)
      throw new InvalidOperationException($"Não é possível abrir um ticket no status '{Status}'.");

    Status = TicketStatus.Aberto;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: Aberto → AguardandoDistribuicao.
  /// </summary>
  public void MakeAvailableForDistribution()
  {
    if (Status != TicketStatus.Aberto)
      throw new InvalidOperationException($"Não é possível colocar em distribuição um ticket no status '{Status}'.");

    Status = TicketStatus.AguardandoDistribuicao;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: AguardandoDistribuicao → DisponibilizadoAoTecnico.
  /// Disponibilizar não é o mesmo que Atribuir — o Técnico ainda precisa aceitar.
  /// </summary>
  public void OfferToTechnician(Guid technicianId)
  {
    if (Status != TicketStatus.AguardandoDistribuicao)
      throw new InvalidOperationException($"Não é possível disponibilizar ao técnico um ticket no status '{Status}'.");

    Status = TicketStatus.DisponibilizadoAoTecnico;
    // Não preenche AssignedTechnicianId aqui — apenas após Aceite
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: DisponibilizadoAoTecnico → Aceito.
  /// Estabelece atribuição (preenche AssignedTechnicianId).
  /// </summary>
  public void Accept(Guid technicianId)
  {
    if (Status != TicketStatus.DisponibilizadoAoTecnico)
      throw new InvalidOperationException($"Não é possível aceitar um ticket no status '{Status}'.");

    Status = TicketStatus.Aceito;
    AssignedTechnicianId = technicianId;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: DisponibilizadoAoTecnico → Recusado.
  /// Ticket volta à fila de distribuição.
  /// </summary>
  public void Refuse(Guid technicianId, string reason)
  {
    if (Status != TicketStatus.DisponibilizadoAoTecnico)
      throw new InvalidOperationException($"Não é possível recusar um ticket no status '{Status}'.");

    if (string.IsNullOrWhiteSpace(reason))
      throw new ArgumentException("Motivo da recusa é obrigatório.", nameof(reason));

    Status = TicketStatus.Recusado;
    RefusalReason = reason;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: Aceito → EmDeslocamento.
  /// </summary>
  public void StartTravel()
  {
    if (Status != TicketStatus.Aceito)
      throw new InvalidOperationException($"Não é possível iniciar deslocamento de um ticket no status '{Status}'.");

    Status = TicketStatus.EmDeslocamento;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: EmDeslocamento → EmAtendimento.
  /// </summary>
  public void StartAttendance()
  {
    if (Status != TicketStatus.EmDeslocamento)
      throw new InvalidOperationException($"Não é possível iniciar atendimento de um ticket no status '{Status}'.");

    Status = TicketStatus.EmAtendimento;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: EmAtendimento → Concluido.
  /// </summary>
  public void Complete()
  {
    if (Status != TicketStatus.EmAtendimento)
      throw new InvalidOperationException($"Não é possível concluir um ticket no status '{Status}'.");

    Status = TicketStatus.Concluido;
    CompletedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Transição: (qualquer estado) → Cancelado.
  /// Pode ser cancelado a qualquer momento.
  /// </summary>
  public void Cancel(string reason)
  {
    if (Status == TicketStatus.Cancelado)
      throw new InvalidOperationException("Ticket já está cancelado.");

    if (Status == TicketStatus.Concluido)
      throw new InvalidOperationException("Não é possível cancelar um ticket concluído.");

    if (string.IsNullOrWhiteSpace(reason))
      throw new ArgumentException("Motivo do cancelamento é obrigatório.", nameof(reason));

    Status = TicketStatus.Cancelado;
    RefusalReason = reason;
    CancelledAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Atribui o Ticket a uma Equipe.
  /// </summary>
  public void AssignToTeam(Guid teamId)
  {
    AssignedTeamId = teamId;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Atualiza o assunto do Ticket.
  /// </summary>
  public void UpdateSubject(string subject)
  {
    if (string.IsNullOrWhiteSpace(subject))
      throw new ArgumentException("Assunto do ticket é obrigatório.", nameof(subject));

    Subject = subject;
    UpdatedAt = DateTime.UtcNow;
  }

  /// <summary>
  /// Atualiza a descrição do Ticket.
  /// </summary>
  public void UpdateDescription(string? description)
  {
    Description = description;
    UpdatedAt = DateTime.UtcNow;
  }
}

/// <summary>
/// Constantes de status do Ticket.
/// </summary>
public static class TicketStatus
{
  public const string Recebido = "recebido";
  public const string Aberto = "aberto";
  public const string AguardandoDistribuicao = "aguardando_distribuicao";
  public const string DisponibilizadoAoTecnico = "disponibilizado_ao_tecnico";
  public const string Aceito = "aceito";
  public const string Recusado = "recusado";
  public const string EmDeslocamento = "em_deslocamento";
  public const string EmAtendimento = "em_atendimento";
  public const string Concluido = "concluido";
  public const string Cancelado = "cancelado";
}
