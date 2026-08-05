namespace ServiZone.Application.Interfaces;

/// <summary>
/// Interface para serviço de notificações push.
/// Envia notificações push para dispositivos móveis dos técnicos.
/// </summary>
public interface IPushNotificationService
{
  /// <summary>
  /// Envia uma notificação push para um dispositivo específico.
  /// </summary>
  /// <param name="deviceToken">Token do dispositivo destinatário (FCM ou APNs).</param>
  /// <param name="title">Título da notificação.</param>
  /// <param name="body">Corpo da notificação.</param>
  /// <param name="data">Dados adicionais (payload).</param>
  /// <param name="cancellationToken">Token de cancelamento.</param>
  Task SendAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);

  /// <summary>
  /// Envia uma notificação push para múltiplos dispositivos.
  /// </summary>
  /// <param name="deviceTokens">Lista de tokens dos dispositivos destinatários.</param>
  /// <param name="title">Título da notificação.</param>
  /// <param name="body">Corpo da notificação.</param>
  /// <param name="data">Dados adicionais (payload).</param>
  /// <param name="cancellationToken">Token de cancelamento.</param>
  Task SendToMultipleAsync(IEnumerable<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null, CancellationToken cancellationToken = default);
}
