namespace ServiZone.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Toda entidade possui um identificador único (UUID).
/// </summary>
public abstract class Entity
{
  /// <summary>
  /// Identificador único da entidade.
  /// UUIDs são sempre gerados pela camada Application antes da persistência.
  /// </summary>
  public Guid Id { get; protected set; }

  /// <summary>
  /// Data de criação do registro.
  /// </summary>
  public DateTime CreatedAt { get; protected set; }

  /// <summary>
  /// Data da última atualização do registro.
  /// </summary>
  public DateTime UpdatedAt { get; protected set; }

  protected Entity()
  {
    Id = Guid.NewGuid();
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  protected Entity(Guid id)
  {
    Id = id;
    CreatedAt = DateTime.UtcNow;
    UpdatedAt = DateTime.UtcNow;
  }

  public override bool Equals(object? obj)
  {
    if (obj is not Entity other)
      return false;

    if (ReferenceEquals(this, other))
      return true;

    if (GetType() != other.GetType())
      return false;

    return Id == other.Id;
  }

  public override int GetHashCode()
  {
    return Id.GetHashCode();
  }

  public static bool operator ==(Entity? left, Entity? right)
  {
    if (left is null && right is null)
      return true;

    if (left is null || right is null)
      return false;

    return left.Equals(right);
  }

  public static bool operator !=(Entity? left, Entity? right)
  {
    return !(left == right);
  }
}
