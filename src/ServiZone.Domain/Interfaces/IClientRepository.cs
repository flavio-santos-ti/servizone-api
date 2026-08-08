using ServiZone.Domain.Entities;

namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Repositório para a entidade Client.
/// </summary>
public interface IClientRepository : IRepository<Client>
{
  /// <summary>
  /// Busca Cliente por número de documento (CPF/CNPJ).
  /// </summary>
  Task<Client?> GetByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Cliente por e-mail.
  /// </summary>
  Task<Client?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

  /// <summary>
  /// Busca Clientes por nome (busca parcial).
  /// </summary>
  Task<IEnumerable<Client>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
}
