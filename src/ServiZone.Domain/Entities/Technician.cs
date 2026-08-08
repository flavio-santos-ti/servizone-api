using ServiZone.Domain.ValueObjects;

namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa um Técnico — profissional que executa Tickets em campo.
/// Possui disponibilidade, localização temporal e raio de atuação.
/// </summary>
public class Technician : TenantEntity
{
  /// <summary>
  /// Nome completo do Técnico.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// E-mail de contato do Técnico.
  /// </summary>
  public string Email { get; private set; } = string.Empty;

  /// <summary>
  /// Telefone de contato do Técnico.
  /// </summary>
  public string? Phone { get; private set; }

  /// <summary>
  /// Status do técnico (active, inactive, unavailable).
  /// </summary>
  public string Status { get; private set; } = "active";

  /// <summary>
  /// Localização atual do Técnico com timestamp de captura.
  /// Informação temporal — deve ser validada antes de usar em distribuição.
  /// </summary>
  public TechnicianLocation? CurrentLocation { get; private set; }

  /// <summary>
  /// Raio máximo de atuação do Técnico (em quilômetros).
  /// Usado na Distribuição Inteligente.
  /// </summary>
  public ServiceRadius? ServiceRadius { get; private set; }

  /// <summary>
  /// Lista de especialidades do Técnico (JSONB).
  /// Ex: ["fiber_optic", "copper_network", "wireless"].
  /// </summary>
  public string Specialties { get; private set; } = "[]";

  /// <summary>
  /// Área geográfica de atuação do Técnico (JSONB).
  /// Ex: lista de bairros, cidades, polígonos, etc.
  /// </summary>
  public string? WorkingArea { get; private set; }

  // Construtor privado para ORM
  private Technician() : base()
  {
  }

  // Construtor para criação de novo Técnico
  public Technician(
    Guid id,
    Guid organizationId,
    string name,
    string email,
    string? phone = null,
    ServiceRadius? serviceRadius = null,
    string specialties = "[]",
    string? workingArea = null)
    : base(id, organizationId)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome do técnico é obrigatório.", nameof(name));

    if (string.IsNullOrWhiteSpace(email))
      throw new ArgumentException("E-mail do técnico é obrigatório.", nameof(email));

    Name = name;
    Email = email;
    Phone = phone?.Trim();
    ServiceRadius = serviceRadius;
    Specialties = specialties;
    WorkingArea = workingArea;
    Status = "active";
  }

  /// <summary>
  /// Atualiza a localização atual do Técnico com timestamp de captura.
  /// </summary>
  /// <param name="coordinates">Coordenadas geográficas capturadas.</param>
  public void UpdateLocation(GeoCoordinates coordinates)
  {
    if (coordinates == null)
      throw new ArgumentNullException(nameof(coordinates));

    CurrentLocation = new TechnicianLocation(coordinates, DateTime.UtcNow);
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome do técnico é obrigatório.", nameof(name));

    Name = name;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateContactInfo(string email, string? phone)
  {
    if (string.IsNullOrWhiteSpace(email))
      throw new ArgumentException("E-mail do técnico é obrigatório.", nameof(email));

    Email = email;
    Phone = phone?.Trim();
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateServiceRadius(ServiceRadius? serviceRadius)
  {
    ServiceRadius = serviceRadius;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateSpecialties(string specialties)
  {
    if (string.IsNullOrWhiteSpace(specialties))
      throw new ArgumentException("Especialidades não podem ser vazias.", nameof(specialties));

    Specialties = specialties;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateWorkingArea(string? workingArea)
  {
    WorkingArea = workingArea;
    UpdatedAt = DateTime.UtcNow;
  }

  public void Activate()
  {
    Status = "active";
    UpdatedAt = DateTime.UtcNow;
  }

  public void Deactivate()
  {
    Status = "inactive";
    UpdatedAt = DateTime.UtcNow;
  }

  public void SetUnavailable()
  {
    Status = "unavailable";
    UpdatedAt = DateTime.UtcNow;
  }
}
