using System.ComponentModel.DataAnnotations;
using MailKit.Security;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace AspNet.Module.Email.Options;

/// <summary>
///     Настройки Smtp
/// </summary>
public class EmailSmtpOptions
{
    /// <summary>
    ///     Сервер
    /// </summary>
    [Required]
    public string Host { get; internal set; } = null!;

    /// <summary>
    ///     Пароль
    /// </summary>
    [Required]
    public string Password { get; internal set; } = null!;

    /// <summary>
    ///     Порт
    /// </summary>
    [Required]
    public int Port { get; internal set; }

    /// <summary>
    ///     Сокет
    /// </summary>
    public SecureSocketOptions Socket { get; internal set; } = SecureSocketOptions.Auto;

    /// <summary>
    ///     Логин
    /// </summary>
    [Required]
    public string Username { get; internal set; } = null!;
}