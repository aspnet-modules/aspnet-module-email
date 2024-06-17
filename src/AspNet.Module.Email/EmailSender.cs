using AspNet.Module.Email.Models;
using AspNet.Module.Email.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace AspNet.Module.Email;

/// <inheritdoc />
public class EmailSender(IOptions<EmailOptions> options, ILogger<EmailSender> logger) : IEmailSender
{
    /// <inheritdoc />
    public async Task<Guid> Send(EmailMessage email, CancellationToken ct = default)
    {
        var optionsValue = options.Value;
        var emailId = email.Id ?? Guid.NewGuid();
        var scope = new Dictionary<string, object>
        {
            { "EmailId", emailId },
            { "Subject", email.Subject },
            { "Senders", string.Join(',', email.Senders) },
            { "Recipients", string.Join(',', email.Recipients) }
        };
        using (logger.BeginScope(scope))
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
                    logger.LogWarning($"Не удалось добавить отправителя {sender}");
                }
            }

            // SMTP пользователь как отправитель
            if (email.SmtpUsernameAsSender)
            {
                if (MailboxAddress.TryParse(optionsValue.Smtp.Username, out var senderAddress))
                {
                    message.From.Add(senderAddress);
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
                    logger.LogWarning($"Не удалось добавить получателя {recipient}");
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
            if (!optionsValue.CheckServerCertificates)
            {
                client.ServerCertificateValidationCallback = (_, _, _, _) => true;
            }

            await client.ConnectAsync(optionsValue.Smtp.Host, optionsValue.Smtp.Port, optionsValue.Smtp.Socket, ct);
            // если есть Username или Password, то авторизуемся
            if (!string.IsNullOrEmpty(optionsValue.Smtp.Username) || !string.IsNullOrEmpty(optionsValue.Smtp.Password))
            {
                await client.AuthenticateAsync(optionsValue.Smtp.Username, optionsValue.Smtp.Password, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);

            using (logger.BeginScope(new Dictionary<string, object> { { "ServerResponse", serverResponse } }))
            {
                logger.LogInformation(
                    $"Письмо «{message.Subject}» отправлено на почту [{string.Join(',', email.Recipients)}]");
            }
        }

        return emailId;
    }
}