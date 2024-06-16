using AspNet.Module.Common;
using AspNet.Module.Email.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.Module.Email;

/// <summary>
///     Модуль Email
/// </summary>
public class EmailModule : IAspNetModule
{
    public void Configure(AspNetModuleContext ctx)
    {
        ctx.Services.Configure<EmailOptions>(
            ctx.Configuration.GetRequiredSection(EmailOptions.Key),
            o => { o.BindNonPublicProperties = true; });
        ctx.Services.AddScoped<IEmailSender, EmailSender>();
    }
}