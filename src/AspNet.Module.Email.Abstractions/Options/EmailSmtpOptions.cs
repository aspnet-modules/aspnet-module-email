using System.Diagnostics.CodeAnalysis;

namespace AspNet.Module.Email.Options;

/// <summary>
///     Настройки Smtp
/// </summary>
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class EmailSmtpOptions
{
    /// <summary>
    ///     Пароль
    /// </summary>
    public string? Password { get; internal set; }

    /// <summary>
    ///     Порт
    /// </summary>
    public int Port { get; internal set; }

    /// <summary>
    ///     Сервер
    /// </summary>
    public string Server { get; internal set; } = null!;

    /// <summary>
    ///     Логин
    /// </summary>
    public string? Username { get; internal set; }
}