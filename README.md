# AspNet.Module.Email

Email sending packages for ASP.NET applications.

## Installation

```sh
# runtime module
dotnet add package AspNet.Module.Email

# abstractions for IEmailSender and related contracts
dotnet add package AspNet.Module.Email.Abstractions
```

## Configuration

- `CheckServerCertificates` controls SMTP certificate validation
- `Smtp.Server` is the SMTP host
- `Smtp.Port` is the SMTP port
- `Smtp.Username` is the login name
- `Smtp.Password` is the password

```json
{
  "Email": {
    "CheckServerCertificates": false,
    "Smtp": {
      "Server": "server",
      "Port": "port",
      "Username": "login",
      "Password": "password"
    }
  }
}
```

## Example

```cs
var email = new EmailMessage(Guid.NewGuid(), "Test message", new[] { from }, new[] { to })
    .WithText("text", false)
    .WithFiles(EmailMessageFile.Excel("report.xlsx", xmlStream));

IEmailSender sender;
await sender.Send(email, default);
```

## Module Registration

```cs
using AspNet.Module.Email;

var builder = AspNetWebApplication.CreateBuilder(args);
builder.RegisterModule<EmailModule>();
```

## Source Code

- Repository: [github.com/aspnet-modules/aspnet-module-email](https://github.com/aspnet-modules/aspnet-module-email)
