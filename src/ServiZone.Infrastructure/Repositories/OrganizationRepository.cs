using Microsoft.EntityFrameworkCore;
using ServiZone.Domain.Entities;
using ServiZone.Domain.Interfaces;
using ServiZone.Infrastructure.Data;

namespace ServiZone.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Organization.
/// </summary>
public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
  public OrganizationRepository(ServiZoneDbContext context) : base(context)
  {
  }

  public async Task<Organization?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _dbSet
        .Where(o => o.Slug == slug)
        .FirstOrDefaultAsync(cancellationToken);
  }

  public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
  {
    return await _dbSet
        .AnyAsync(o => o.Slug == slug, cancellationToken);
  }
}
