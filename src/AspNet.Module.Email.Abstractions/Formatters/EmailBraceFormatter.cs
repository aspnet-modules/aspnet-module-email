using System.Reflection;
using System.Text;

namespace AspNet.Module.Email.Formatters;

/// <summary>
///     Форматирование Email по шаблону {}
/// </summary>
public static class EmailBraceFormatter
{
    /// <summary>
    ///     Подмена текста формата {foo}
    /// </summary>
    public static string Render(ref string formatString, object? injectionObject)
    {
        var strBuilder = new StringBuilder(formatString);
        var attributes = GetPropertyAttributes(injectionObject);
        foreach (var attributeKey in attributes.Keys)
        {
            strBuilder.Replace($"{{{attributeKey}}}", attributes[attributeKey]);
        }

        return strBuilder.ToString();
    }

    private static IDictionary<string, string?> GetPropertyAttributes(object? properties)
    {
        var values = new Dictionary<string, string?>();
        if (properties == null)
        {
            return values;
        }

        var typeInfo = properties.GetType().GetTypeInfo();
        foreach (var propInfo in typeInfo.DeclaredProperties)
        {
            values.Add(propInfo.Name, propInfo.GetValue(properties)?.ToString());
        }

        return values;
    }
}