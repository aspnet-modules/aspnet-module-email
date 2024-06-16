namespace AspNet.Module.Email.Models;

/// <summary>
///     Объект в Email
/// </summary>
public interface IEmailEntity
{
    /// <summary>
    ///     Порядок вложения
    /// </summary>
    int Order { get; }
}