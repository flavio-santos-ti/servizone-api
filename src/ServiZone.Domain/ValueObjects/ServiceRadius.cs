namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o raio de atuação de um Técnico (em quilômetros).
/// Usado na Distribuição Inteligente para determinar se um Técnico pode atender um Ticket.
/// </summary>
public record ServiceRadius
{
  public double RadiusInKilometers { get; init; }

  public ServiceRadius(double radiusInKilometers)
  {
    if (radiusInKilometers <= 0)
      throw new ArgumentException("Raio de atendimento deve ser maior que zero.", nameof(radiusInKilometers));

    RadiusInKilometers = radiusInKilometers;
  }
}
