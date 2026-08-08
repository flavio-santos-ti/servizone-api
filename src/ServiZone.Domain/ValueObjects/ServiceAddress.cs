namespace ServiZone.Domain.ValueObjects;

/// <summary>
/// Value Object que representa o endereço congelado do Local de Atendimento.
/// O endereço é capturado no momento da criação do Ticket e não muda, mesmo se o cadastro do Cliente for alterado.
/// </summary>
public record ServiceAddress
{
  public string Street { get; init; }
  public string Number { get; init; }
  public string? Complement { get; init; }
  public string Neighborhood { get; init; }
  public string City { get; init; }
  public string State { get; init; }
  public string PostalCode { get; init; }
  public string Country { get; init; }
  public GeoCoordinates? Coordinates { get; init; }

  public ServiceAddress(
    string street,
    string number,
    string? complement,
    string neighborhood,
    string city,
    string state,
    string postalCode,
    string country,
    GeoCoordinates? coordinates = null)
  {
    if (string.IsNullOrWhiteSpace(street))
      throw new ArgumentException("Logradouro não pode ser vazio.", nameof(street));

    if (string.IsNullOrWhiteSpace(number))
      throw new ArgumentException("Número não pode ser vazio.", nameof(number));

    if (string.IsNullOrWhiteSpace(neighborhood))
      throw new ArgumentException("Bairro não pode ser vazio.", nameof(neighborhood));

    if (string.IsNullOrWhiteSpace(city))
      throw new ArgumentException("Cidade não pode ser vazia.", nameof(city));

    if (string.IsNullOrWhiteSpace(state))
      throw new ArgumentException("Estado não pode ser vazio.", nameof(state));

    if (string.IsNullOrWhiteSpace(postalCode))
      throw new ArgumentException("CEP não pode ser vazio.", nameof(postalCode));

    if (string.IsNullOrWhiteSpace(country))
      throw new ArgumentException("País não pode ser vazio.", nameof(country));

    Street = street.Trim();
    Number = number.Trim();
    Complement = complement?.Trim();
    Neighborhood = neighborhood.Trim();
    City = city.Trim();
    State = state.Trim();
    PostalCode = postalCode.Trim();
    Country = country.Trim();
    Coordinates = coordinates;
  }
}
