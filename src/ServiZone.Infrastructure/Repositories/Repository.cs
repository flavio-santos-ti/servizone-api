using Microsoft.EntityFrameworkCore;
using ServiZone.Domain.Entities;
using ServiZone.Domain.Interfaces;
using ServiZone.Infrastructure.Data;

namespace ServiZone.Infrastructure.Repositories;

/// <summary>
/// Implementação base de repositório usando EF Core.
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
  protected readonly ServiZoneDbContext _context;
  protected readonly DbSet<T> _dbSet;

  public Repository(ServiZoneDbContext context)
  {
    _context = context;
    _dbSet = context.Set<T>();
  }

  public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
  {
    return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
  }

  public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
  {
    return await _dbSet.ToListAsync(cancellationToken);
  }

  public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
  {
    await _dbSet.AddAsync(entity, cancellationToken);
  }

  public virtual void Update(T entity)
  {
    _dbSet.Update(entity);
  }

  public virtual void Remove(T entity)
  {
    _dbSet.Remove(entity);
  }

  public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await _context.SaveChangesAsync(cancellationToken);
  }
}
