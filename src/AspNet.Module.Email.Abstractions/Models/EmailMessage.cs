using MimeKit;

namespace AspNet.Module.Email.Models;

/// <summary>
///     Данные сообщение Email
/// </summary>
/// <param name="Subject">Тема</param>
/// <param name="Senders">Отправители</param>
/// <param name="Recipients">Получатели</param>
/// <param name="Entities">Объекты в Email</param>
public record EmailMessage(string Subject, string[] Senders, string[] Recipients, params IEmailEntity[] Entities)
{
    /// <summary>
    ///     ИД Email
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Guid? Id { get; set; } = null!;

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
}