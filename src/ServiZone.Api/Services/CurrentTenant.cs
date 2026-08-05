using ServiZone.Domain.Interfaces;

namespace ServiZone.Api.Services;

/// <summary>
/// Implementação do serviço de resolução do tenant atual.
/// Extrai o OrganizationId do contexto HTTP (JWT).
/// </summary>
public class CurrentTenant : ICurrentTenant
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public CurrentTenant(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Guid OrganizationId
  {
    get
    {
      var httpContext = _httpContextAccessor.HttpContext;
      if (httpContext == null)
        return Guid.Empty;

      var organizationIdClaim = httpContext.User.FindFirst("org")?.Value;
      if (string.IsNullOrEmpty(organizationIdClaim))
        return Guid.Empty;

      return Guid.TryParse(organizationIdClaim, out var orgId) ? orgId : Guid.Empty;
    }
  }

  public bool IsAvailable => OrganizationId != Guid.Empty;
}
