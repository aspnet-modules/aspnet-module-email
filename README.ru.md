# Модуль для отправки Email

Модуль для отправки Email

```sh
dotnet add package AspNet.Module.Email
// для доступа к IEmailSender
dotnet add package AspNet.Module.Email.Abstractions
```

## Конфигурация

- **CheckServerCertificates** - Проверять сертификаты сервера
- **Smtp** - Настройки Smtp
    - **Server** - Сервер
    - **Port** - Порт
    - **Username** - Логин
    - **Password** - Пароль

```json
{
  "Email": {
    "CheckServerCertificates": "false",
    "Smtp": {
      "Server": "server",
      "Port": "port",
      "Username": "login",
      "Password": "password"
    }
  }
}
```

## Пример

```cs
var email = new EmailMessage(Guid.NewGuid(), "Тестовое сообщение", new[] { from }, new[] { to })
    .WithText("text", false)
    .WithFiles(EmailMessageFile.Excel("report.xlsx", xmlStream));

IEmailSender sender;
await sender.Send(email, default);
```

## Регистрация модуля

```cs
using AspNet.Module.Email;

var builder = AspNetWebApplication.CreateBuilder(args);

// для отправки Email
builder.RegisterModule<EmailModule>();
```