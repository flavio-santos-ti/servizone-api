namespace ServiZone.Application.Dtos;

/// <summary>
/// DTO base para respostas.
/// </summary>
public abstract class ResponseDto
{
  /// <summary>
  /// Identificador único do recurso.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Data de criação do recurso.
  /// </summary>
  public DateTime CreatedAt { get; set; }

  /// <summary>
  /// Data da última atualização do recurso.
  /// </summary>
  public DateTime UpdatedAt { get; set; }
}
