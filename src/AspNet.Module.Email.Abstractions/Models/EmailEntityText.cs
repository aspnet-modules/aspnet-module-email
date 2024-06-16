namespace AspNet.Module.Email.Models;

/// <summary>
///     Текст в Email
/// </summary>
/// <param name="Text">Текст</param>
/// <param name="IsHtml">Html?</param>
public sealed record EmailEntityText(string Text, bool IsHtml, int Order = 1) : IEmailEntity;