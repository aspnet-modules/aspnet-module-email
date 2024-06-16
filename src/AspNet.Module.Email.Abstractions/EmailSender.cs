using System.Runtime.CompilerServices;
using AspNet.Module.Email.Models;
using AspNet.Module.Email.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

[assembly: InternalsVisibleTo("AspNet.Module.Email.Tests")]

namespace AspNet.Module.Email;

/// <inheritdoc />
public class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;

    /// <summary>
    ///     Создание сендера
    /// </summary>
    public EmailSender(IOptions<EmailOptions> options, ILogger<EmailSender> logger)
    {
        _options = options.Value;
        Logger = logger;
    }

    private ILogger<EmailSender> Logger { get; }

    /// <inheritdoc />
    public async Task<Guid> Send(EmailMessage email, CancellationToken ct = default)
    {
        var emailId = email.Id ?? Guid.NewGuid();
        var scope = new Dictionary<string, object>
        {
            { "EmailId", emailId },
            { "Subject", email.Subject },
            { "Senders", string.Join(',', email.Senders) },
            { "Recipients", string.Join(',', email.Recipients) }
        };
        using (Logger.BeginScope(scope))
        {
            var message = new MimeMessage();

            // Отправители
            foreach (var sender in email.Senders)
            {
                if (MailboxAddress.TryParse(sender, out var senderAddress))
                {
                    message.From.Add(senderAddress);
                }
                else
                {
                    Logger.LogWarning($"Не удалось добавить отправителя {sender}");
                }
            }

            // Получатели
            foreach (var recipient in email.Recipients)
            {
                if (MailboxAddress.TryParse(recipient, out var recipientAddress))
                {
                    message.To.Add(recipientAddress);
                }
                else
                {
                    Logger.LogWarning($"Не удалось добавить получателя {recipient}");
                }
            }

            // Тема
            message.Subject = email.Subject;
            // Данные
            message.Body = await email.BuildBody(ct);

            using var client = new SmtpClient();
            var serverResponse = string.Empty;
            client.MessageSent += (_, e) => { serverResponse = e.Response; };

            // Пропускаем валидацию сертификатов сервера
            if (!_options.CheckServerCertificates)
            {
                client.ServerCertificateValidationCallback = (_, _, _, _) => true;
            }

            await client.ConnectAsync(_options.Smtp.Server, _options.Smtp.Port, SecureSocketOptions.Auto, ct);
            // если есть Username или Password, то авторизуемся
            if (!string.IsNullOrEmpty(_options.Smtp.Username) || !string.IsNullOrEmpty(_options.Smtp.Password))
            {
                await client.AuthenticateAsync(_options.Smtp.Username, _options.Smtp.Password, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            using (Logger.BeginScope(new Dictionary<string, object> { { "ServerResponse", serverResponse } }))
            {
                Logger.LogInformation(
                    $"Письмо «{message.Subject}» отправлено на почту [{string.Join(',', email.Recipients)}]");
            }
        }

        return emailId;
    }
}