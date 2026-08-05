namespace ServiZone.Api.Middleware;

/// <summary>
/// Middleware para adicionar CorrelationId em todas as requisições.
/// O CorrelationId é usado para rastreamento de logs e troubleshooting.
/// </summary>
public class CorrelationIdMiddleware
{
  private readonly RequestDelegate _next;
  private const string CorrelationIdHeaderName = "X-Correlation-Id";

  public CorrelationIdMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    // Tenta obter o CorrelationId do header da requisição
    var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

    // Se não houver, gera um novo
    if (string.IsNullOrEmpty(correlationId))
    {
      correlationId = Guid.NewGuid().ToString();
    }

    // Adiciona o CorrelationId no contexto para uso posterior
    context.Items["CorrelationId"] = correlationId;

    // Adiciona o CorrelationId no header da resposta
    context.Response.Headers.Append(CorrelationIdHeaderName, correlationId);

    await _next(context);
  }
}

/// <summary>
/// Métodos de extensão para registrar o middleware de CorrelationId.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
  public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
  {
    return builder.UseMiddleware<CorrelationIdMiddleware>();
  }
}
