namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa uma Organização (tenant) na plataforma ServiZone.
/// Cada Organização é um cliente contratante da plataforma.
/// Todos os dados operacionais estão vinculados a uma Organização.
/// </summary>
public class Organization : Entity
{
  /// <summary>
  /// Nome da organização (razão social ou nome fantasia).
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Slug único da organização, usado em URLs e logs.
  /// </summary>
  public string Slug { get; private set; } = string.Empty;

  /// <summary>
  /// Status da organização (active | inactive).
  /// Controla o acesso à plataforma.
  /// </summary>
  public string Status { get; private set; } = "active";

  /// <summary>
  /// Configurações operacionais da organização (JSONB).
  /// Ex: raio padrão de atendimento, campos obrigatórios personalizados, etc.
  /// </summary>
  public string Config { get; private set; } = "{}";

  // Construtor privado para ORM
  private Organization() : base()
  {
  }

  // Construtor para criação de nova organização
  public Organization(Guid id, string name, string slug) : base(id)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome da organização é obrigatório.", nameof(name));

    if (string.IsNullOrWhiteSpace(slug))
      throw new ArgumentException("Slug da organização é obrigatório.", nameof(slug));

    Name = name;
    Slug = slug;
    Status = "active";
    Config = "{}";
  }

  public void UpdateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome da organização é obrigatório.", nameof(name));

    Name = name;
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

  public void UpdateConfig(string config)
  {
    if (string.IsNullOrWhiteSpace(config))
      throw new ArgumentException("Configuração não pode ser vazia.", nameof(config));

    Config = config;
    UpdatedAt = DateTime.UtcNow;
  }
}
