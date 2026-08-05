namespace ServiZone.Application.Interfaces;

/// <summary>
/// Interface para serviço de geocodificação de endereços.
/// Converte endereços textuais em coordenadas geográficas (latitude/longitude).
/// </summary>
public interface IGeocoder
{
  /// <summary>
  /// Geocodifica um endereço textual.
  /// </summary>
  /// <param name="address">Endereço completo no formato de texto.</param>
  /// <param name="cancellationToken">Token de cancelamento.</param>
  /// <returns>Coordenadas geográficas (latitude, longitude) ou null se não encontrado.</returns>
  Task<(double Latitude, double Longitude)?> GeocodeAsync(string address, CancellationToken cancellationToken = default);
}
