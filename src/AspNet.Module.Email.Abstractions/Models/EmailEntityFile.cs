using MimeKit;

namespace AspNet.Module.Email.Models;

/// <summary>
///     Данные файла
/// </summary>
/// <param name="Name">Название файла</param>
/// <param name="Data">Данные файла</param>
/// <param name="ContentType">Тип файла</param>
public sealed record EmailEntityFile(string Name, Stream Data, ContentType ContentType, int Order = 2) : IEmailEntity
{
    /// <summary>
    ///     Csv
    /// </summary>
    /// <param name="name">Название файла</param>
    /// <param name="data">Данные файла</param>
    public static EmailEntityFile Csv(string name, Stream data) =>
        new(name, data, new ContentType("text", "csv"));

    /// <summary>
    ///     Excel
    /// </summary>
    /// <param name="name">Название файла</param>
    /// <param name="data">Данные файла</param>
    public static EmailEntityFile Excel(string name, Stream data) =>
        new(name, data, new ContentType("text", "xlsx"));

    /// <summary>
    ///     Pdf
    /// </summary>
    /// <param name="name">Название файла</param>
    /// <param name="data">Данные файла</param>
    public static EmailEntityFile Pdf(string name, Stream data) =>
        new(name, data, new ContentType("application", "pdf"));

    /// <summary>
    ///     Stream
    /// </summary>
    /// <param name="name">Название файла</param>
    /// <param name="data">Данные файла</param>
    public static EmailEntityFile Stream(string name, Stream data) =>
        new(name, data, new ContentType("application", "octet-stream"));

    /// <summary>
    ///     Text
    /// </summary>
    /// <param name="name">Название файла</param>
    /// <param name="data">Данные файла</param>
    public static EmailEntityFile Text(string name, Stream data) =>
        new(name, data, new ContentType("text", "txt"));
}