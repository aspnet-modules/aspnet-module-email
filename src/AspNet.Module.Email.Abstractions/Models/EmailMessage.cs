using MimeKit;

namespace AspNet.Module.Email.Models;

/// <summary>
///     Данные сообщение Email
/// </summary>
public record EmailMessage
{
    /// <summary>
    ///     Данные сообщение Email
    /// </summary>
    private EmailMessage(string subject, string[] senders, string[] recipients, params IEmailEntity[] entities)
    {
        Subject = subject;
        Senders = senders;
        SmtpUsernameAsSender = false;
        Recipients = recipients;
        Entities = entities;
    }

    private EmailMessage(string subject, string[] recipients, params IEmailEntity[] entities)
    {
        Subject = subject;
        Senders = [];
        SmtpUsernameAsSender = true;
        Recipients = recipients;
        Entities = entities;
    }

    /// <summary>
    ///     Пользователь SMTP как отправитель
    /// </summary>
    internal bool SmtpUsernameAsSender { get; }

    /// <summary>Объекты в Email</summary>
    public IEmailEntity[] Entities { get; }

    /// <summary>
    ///     ИД Email
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid? Id { get; set; } = null!;

    /// <summary>Получатели</summary>
    public string[] Recipients { get; }

    /// <summary>Отправители</summary>
    public string[] Senders { get; }

    /// <summary>Тема</summary>
    public string Subject { get; }

    // ReSharper disable once UnusedParameter.Global
    protected virtual Task<MimeEntity> ResolveFallbackEntity(IEmailEntity entity, CancellationToken ct) =>
        throw new NotImplementedException();

    /// <summary>
    ///     Тело сообщения
    /// </summary>
    internal async Task<MimeEntity> BuildBody(CancellationToken ct)
    {
        var parts = new List<MimeEntity>();

        // text
        var orderedEntities = Entities.OrderBy(x => x.Order);
        foreach (var entity in orderedEntities)
        {
            switch (entity)
            {
                case EmailEntityText entityText:
                {
                    parts.Add(CreateTextMimeType(entityText.Text, entityText.IsHtml));
                    break;
                }
                case EmailEntityFile entityFile:
                    parts.Add(CreateAttachmentMimeType(entityFile.Name, entityFile.Data, entityFile.ContentType));
                    break;
                default:
                    parts.Add(await ResolveFallbackEntity(entity, ct));
                    break;
            }
        }

        var body = new Multipart("mixed");
        foreach (var part in parts)
        {
            body.Add(part);
        }

        return body;
    }

    protected static MimePart CreateAttachmentMimeType(string name, Stream data, ContentType contentType) =>
        new(contentType)
        {
            Content = new MimeContent(data),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = name
        };

    protected static MimePart CreateTextMimeType(string text, bool isHtml) =>
        new TextPart(isHtml ? "html" : "plain")
        {
            Text = text
        };

    public static EmailMessage WithSenders(string subject, string[] senders, string[] recipients,
        params IEmailEntity[] entities) => new(subject, senders, recipients, entities);

    public static EmailMessage WithSmtpUsernameAsSender(string subject, string[] recipients,
        params IEmailEntity[] entities) => new(subject, recipients, entities);
}