namespace ServiZone.Api.Configuration;

/// <summary>
/// Opções de configuração para o túnel SSH (Development only).
/// </summary>
public class SshTunnelOptions
{
  /// <summary>
  /// Indica se o túnel SSH está habilitado.
  /// Deve ser true apenas em ambiente de Development.
  /// </summary>
  public bool Enabled { get; set; }

  /// <summary>
  /// Host do servidor SSH (IP ou hostname).
  /// </summary>
  public string SshHost { get; set; } = string.Empty;

  /// <summary>
  /// Porta do servidor SSH (padrão: 22).
  /// </summary>
  public int SshPort { get; set; } = 22;

  /// <summary>
  /// Nome de usuário SSH.
  /// </summary>
  public string SshUsername { get; set; } = string.Empty;

  /// <summary>
  /// Caminho completo para a chave privada SSH (ed25519 ou RSA).
  /// </summary>
  public string SshPrivateKeyPath { get; set; } = string.Empty;

  /// <summary>
  /// Passphrase da chave privada (se houver).
  /// </summary>
  public string? SshPassphrase { get; set; }

  /// <summary>
  /// Host remoto a ser tunelado (geralmente 127.0.0.1 do servidor remoto).
  /// </summary>
  public string RemoteHost { get; set; } = "127.0.0.1";

  /// <summary>
  /// Porta remota do PostgreSQL (padrão: 5432).
  /// </summary>
  public int RemotePort { get; set; } = 5432;

  /// <summary>
  /// Porta local para onde o túnel PostgreSQL será mapeado (padrão: 15432).
  /// </summary>
  public uint LocalPort { get; set; } = 15432;

  /// <summary>
  /// Porta remota do Redis (padrão: 6379).
  /// </summary>
  public int RedisRemotePort { get; set; } = 6379;

  /// <summary>
  /// Porta local para onde o túnel Redis será mapeado (padrão: 6379).
  /// </summary>
  public uint RedisLocalPort { get; set; } = 6379;
}
