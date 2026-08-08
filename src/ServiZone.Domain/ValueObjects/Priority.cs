namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa a prioridade de um Ticket.
/// Valores permitidos: low, normal, high, urgent.
/// </summary>
public record Priority
{
  private static readonly string[] ValidValues = { "low", "normal", "high", "urgent" };

  public string Value { get; init; }

  public Priority(string value)
  {
    if (string.IsNullOrWhiteSpace(value))
      throw new ArgumentException("Prioridade não pode ser vazia.", nameof(value));

    var normalizedValue = value.ToLowerInvariant();
    if (!ValidValues.Contains(normalizedValue))
      throw new ArgumentException($"Prioridade inválida. Valores permitidos: {string.Join(", ", ValidValues)}.", nameof(value));

    Value = normalizedValue;
  }

  public static Priority Low => new("low");
  public static Priority Normal => new("normal");
  public static Priority High => new("high");
  public static Priority Urgent => new("urgent");
}
