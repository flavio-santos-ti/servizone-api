namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa coordenadas geográficas (latitude e longitude).
/// Imutável — qualquer alteração requer criação de nova instância.
/// </summary>
public record GeoCoordinates
{
  public double Latitude { get; init; }
  public double Longitude { get; init; }

  public GeoCoordinates(double latitude, double longitude)
  {
    if (latitude < -90 || latitude > 90)
      throw new ArgumentException("Latitude deve estar entre -90 e 90.", nameof(latitude));

    if (longitude < -180 || longitude > 180)
      throw new ArgumentException("Longitude deve estar entre -180 e 180.", nameof(longitude));

    Latitude = latitude;
    Longitude = longitude;
  }
}
