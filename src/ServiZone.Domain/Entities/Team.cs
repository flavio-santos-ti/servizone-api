namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa uma Equipe — conjunto de Técnicos que opera de forma coordenada.
/// Possui especialidades e área de atuação.
/// </summary>
public class Team : TenantEntity
{
  /// <summary>
  /// Nome da Equipe.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Status da equipe (active, inactive).
  /// </summary>
  public string Status { get; private set; } = "active";

  /// <summary>
  /// Lista de especialidades da Equipe (JSONB).
  /// Ex: ["fiber_optic", "copper_network", "wireless"].
  /// </summary>
  public string Specialties { get; private set; } = "[]";

  /// <summary>
  /// Área geográfica de atuação da Equipe (JSONB).
  /// Ex: lista de bairros, cidades, polígonos, etc.
  /// </summary>
  public string? WorkingArea { get; private set; }

  // Construtor privado para ORM
  private Team() : base()
  {
  }

  // Construtor para criação de nova Equipe
  public Team(
    Guid id,
    Guid organizationId,
    string name,
    string specialties = "[]",
    string? workingArea = null)
    : base(id, organizationId)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome da equipe é obrigatório.", nameof(name));

    Name = name;
    Specialties = specialties;
    WorkingArea = workingArea;
    Status = "active";
  }

  public void UpdateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome da equipe é obrigatório.", nameof(name));

    Name = name;
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
}
