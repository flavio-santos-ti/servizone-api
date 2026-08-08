namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa uma Integração — configuração de comunicação entre Organização e sistema externo.
/// Possui API Key para autenticação na API Externa.
/// </summary>
public class Integration : TenantEntity
{
  /// <summary>
  /// Nome da Integração.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Tipo do sistema externo (erp, crm, itsm, custom).
  /// </summary>
  public string SystemType { get; private set; } = string.Empty;

  /// <summary>
  /// Status da integração (active, inactive).
  /// </summary>
  public string Status { get; private set; } = "active";

  /// <summary>
  /// Hash da API Key para autenticação (BCrypt).
  /// A API Key original nunca é armazenada.
  /// </summary>
  public string ApiKeyHash { get; private set; } = string.Empty;

  /// <summary>
  /// Configurações específicas da integração (JSONB).
  /// Ex: URL do webhook, campos personalizados, mapeamentos, etc.
  /// </summary>
  public string Config { get; private set; } = "{}";

  // Construtor privado para ORM
  private Integration() : base()
  {
  }

  // Construtor para criação de nova Integração
  public Integration(
    Guid id,
    Guid organizationId,
    string name,
    string systemType,
    string apiKeyHash,
    string config = "{}")
    : base(id, organizationId)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome da integração é obrigatório.", nameof(name));

    if (string.IsNullOrWhiteSpace(systemType))
      throw new ArgumentException("Tipo do sistema externo é obrigatório.", nameof(systemType));

    if (string.IsNullOrWhiteSpace(apiKeyHash))
      throw new ArgumentException("Hash da API Key é obrigatório.", nameof(apiKeyHash));

    Name = name;
    SystemType = systemType.ToLowerInvariant();
    ApiKeyHash = apiKeyHash;
    Config = config;
    Status = "active";
  }

  /// <summary>
  /// Gera uma nova API Key.
  /// A API Key original deve ser retornada ao cliente e não será mais recuperável.
  /// O hash deve ser calculado na Application layer e armazenado via SetApiKeyHash().
  /// </summary>
  /// <returns>A API Key gerada (deve ser retornada ao cliente uma única vez).</returns>
  public string GenerateApiKey()
  {
    var apiKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    UpdatedAt = DateTime.UtcNow;
    return apiKey;
  }

  /// <summary>
  /// Armazena o hash de uma API Key.
  /// O hash deve ser calculado na Application layer (com BCrypt).
  /// </summary>
  /// <param name="hash">Hash BCrypt da API Key.</param>
  public void SetApiKeyHash(string hash)
  {
    if (string.IsNullOrWhiteSpace(hash))
      throw new ArgumentException("Hash da API Key não pode ser vazio.", nameof(hash));

    ApiKeyHash = hash;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome da integração é obrigatório.", nameof(name));

    Name = name;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateConfig(string config)
  {
    if (string.IsNullOrWhiteSpace(config))
      throw new ArgumentException("Configuração não pode ser vazia.", nameof(config));

    Config = config;
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
