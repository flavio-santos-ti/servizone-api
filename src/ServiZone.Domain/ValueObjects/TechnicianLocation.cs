namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa a localização temporal de um Técnico.
/// Combina coordenadas geográficas com timestamp de captura.
/// A localização é considerada obsoleta (stale) se ultrapassar um threshold temporal.
/// </summary>
public record TechnicianLocation
{
  public GeoCoordinates Coordinates { get; init; }
  public DateTime CapturedAt { get; init; }

  public TechnicianLocation(GeoCoordinates coordinates, DateTime capturedAt)
  {
    Coordinates = coordinates ?? throw new ArgumentNullException(nameof(coordinates));
    CapturedAt = capturedAt;
  }

  /// <summary>
  /// Verifica se a localização está obsoleta (stale) de acordo com o threshold informado.
  /// </summary>
  /// <param name="threshold">Período máximo de validade da localização.</param>
  /// <returns>True se a localização está obsoleta, False caso contrário.</returns>
  public bool IsStale(TimeSpan threshold)
  {
    return DateTime.UtcNow - CapturedAt > threshold;
  }
}
