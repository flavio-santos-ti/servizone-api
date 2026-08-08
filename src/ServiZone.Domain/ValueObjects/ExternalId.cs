namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o identificador de um recurso em um sistema externo.
/// Combina o tipo do sistema de origem com o identificador único naquele sistema.
/// </summary>
public record ExternalId
{
  public string SystemType { get; init; }
  public string Value { get; init; }

  public ExternalId(string systemType, string value)
  {
    if (string.IsNullOrWhiteSpace(systemType))
      throw new ArgumentException("Tipo do sistema externo não pode ser vazio.", nameof(systemType));

    if (string.IsNullOrWhiteSpace(value))
      throw new ArgumentException("Identificador externo não pode ser vazio.", nameof(value));

    SystemType = systemType.Trim();
    Value = value.Trim();
  }
}
