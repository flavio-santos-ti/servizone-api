using ServiZone.Domain.ValueObjects;

namespace ServiZone.Domain.Entities;

/// <summary>
/// Representa um Cliente — destinatário do serviço operacional.
/// Pertence a uma Organização e possui um endereço padrão.
/// </summary>
public class Client : TenantEntity
{
  /// <summary>
  /// Nome ou razão social do Cliente.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Número de documento do Cliente (CPF/CNPJ).
  /// </summary>
  public string? DocumentNumber { get; private set; }

  /// <summary>
  /// E-mail de contato do Cliente.
  /// </summary>
  public string? Email { get; private set; }

  /// <summary>
  /// Telefone de contato do Cliente.
  /// </summary>
  public string? Phone { get; private set; }

  /// <summary>
  /// Endereço padrão do Cliente.
  /// Usado como base para criação de novos Tickets, mas o endereço no Ticket é congelado.
  /// </summary>
  public ServiceAddress? DefaultAddress { get; private set; }

  // Construtor privado para ORM
  private Client() : base()
  {
  }

  // Construtor para criação de novo Cliente
  public Client(
    Guid id,
    Guid organizationId,
    string name,
    string? documentNumber = null,
    string? email = null,
    string? phone = null,
    ServiceAddress? defaultAddress = null)
    : base(id, organizationId)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome do cliente é obrigatório.", nameof(name));

    Name = name;
    DocumentNumber = documentNumber?.Trim();
    Email = email?.Trim();
    Phone = phone?.Trim();
    DefaultAddress = defaultAddress;
  }

  public void UpdateName(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Nome do cliente é obrigatório.", nameof(name));

    Name = name;
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateDocumentNumber(string? documentNumber)
  {
    DocumentNumber = documentNumber?.Trim();
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateContactInfo(string? email, string? phone)
  {
    Email = email?.Trim();
    Phone = phone?.Trim();
    UpdatedAt = DateTime.UtcNow;
  }

  public void UpdateDefaultAddress(ServiceAddress? address)
  {
    DefaultAddress = address;
    UpdatedAt = DateTime.UtcNow;
  }
}
