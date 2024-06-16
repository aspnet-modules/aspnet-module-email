using AspNet.Module.Email.Models;

namespace AspNet.Module.Email;

/// <summary>
///     Сервис отправки Email
/// </summary>
public interface IEmailSender
{
    /// <summary>
    ///     Отправка Email
    /// </summary>
    Task<Guid> Send(EmailMessage email, CancellationToken ct = default);
}