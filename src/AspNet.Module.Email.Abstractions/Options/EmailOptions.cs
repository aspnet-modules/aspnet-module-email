// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace AspNet.Module.Email.Options;

/// <summary>
///     Настройки Email
/// </summary>
public class EmailOptions
{
    /// <summary>
    ///     Проверять сертификаты сервера
    /// </summary>
    public bool CheckServerCertificates { get; internal set; }

    /// <summary>
    ///     Настройки Smtp
    /// </summary>
    public EmailSmtpOptions Smtp { get; internal set; } = null!;
    
    /// <summary>
    ///     От кого по умолчанию
    /// </summary>
    public string? DefaultFrom { get; internal set; }

    /// <summary>
    ///     Ключ модуля Email
    /// </summary>
    public static string Key => "Email";
}