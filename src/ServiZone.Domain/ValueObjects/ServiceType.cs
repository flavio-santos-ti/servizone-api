namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o tipo de serviço de um Ticket.
/// Exemplos: installation, maintenance, inspection, support.
/// </summary>
public record ServiceType
{
  public string Value { get; init; }

  public ServiceType(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
      throw new ArgumentException("Tipo de serviço não pode ser vazio.", nameof(value));

    Value = value.Trim();
  }
}
