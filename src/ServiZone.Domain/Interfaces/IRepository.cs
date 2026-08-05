namespace ServiZone.Domain.Interfaces;

/// <summary>
/// Interface base para todos os repositórios de entidades.
/// </summary>
/// <typeparam name="T">Tipo da entidade gerenciada pelo repositório.</typeparam>
public interface IRepository<T> where T : class
{
  /// <summary>
  /// Busca uma entidade por seu identificador.
  /// </summary>
  Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

  /// <summary>
  /// Obtém todas as entidades.
  /// </summary>
  Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

  /// <summary>
  /// Adiciona uma nova entidade.
  /// </summary>
  Task AddAsync(T entity, CancellationToken cancellationToken = default);

  /// <summary>
  /// Atualiza uma entidade existente.
  /// </summary>
  void Update(T entity);

  /// <summary>
  /// Remove uma entidade.
  /// </summary>
  void Remove(T entity);

  /// <summary>
  /// Salva todas as alterações pendentes no contexto.
  /// </summary>
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
