namespace AspNet.Module.Email;

/// <summary>
///     Рендер шаблона Razor View в HTML для отправки почты.
/// </summary>
public interface IRazorViewToEmailHtmlRenderer
{
    Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, CancellationToken ct);
}