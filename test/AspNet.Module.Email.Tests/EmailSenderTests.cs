using AspNet.Module.Email.Models;
using AspNet.Module.Email.Options;
using AspNet.Module.Test.Helpers;
using AspNet.Module.Test.Loggers;
using AspNet.Module.Test.Unit;
using Microsoft.Extensions.Options;
using netDumbster.smtp;
using Shouldly;
using Xunit.Abstractions;

namespace AspNet.Module.Email.Tests;

/// <summary>
///     Тесты для <see cref="EmailSender" />
/// </summary>
[Collection(EmailFixtureCollection.Name)]
public class EmailSenderTests : BaseIocUnitTests<EmailTestFixture>
{
    private const string SmtpHost = "localhost";
    private const int SmtpPort = 9009;
    private readonly SimpleSmtpServer _smtpServer;

    public EmailSenderTests(EmailTestFixture fixture, ITestOutputHelper testOutputHelper)
        : base(fixture, testOutputHelper)
    {
        _smtpServer ??= SimpleSmtpServer.Start(SmtpPort);
    }

    private IOptions<EmailOptions> Options { get; set; } = null!;

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        _smtpServer.Dispose();
    }

    public override Task InitializeAsync()
    {
        var emailOptions = new EmailOptions
        {
            Smtp = new EmailSmtpOptions
            {
                Password = null,
                Username = null,
                Port = SmtpPort,
                Server = SmtpHost
            }
        };
        Options = new OptionsWrapper<EmailOptions>(emailOptions);
        return base.InitializeAsync();
    }

    [Fact]
    public async Task Should_Send_File_Only()
    {
        // Arrange
        var from = Guid.NewGuid().ToString("N") + "@test.ru";
        var to = Guid.NewGuid().ToString("N") + "@test.ru";
        var logger = new XunitLogger<EmailSender>(TestOutputHelper);
        var sender = new EmailSender(Options, logger);
        var fileStream = (MemoryStream)TestFormFile.CreateTextFile().OpenReadStream();
        var email = new EmailMessage(
            "Тестовое сообщение",
            new[] { from }, new[] { to },
            new EmailEntityText("test text", false),
            EmailEntityFile.Excel("report.xlsx", fileStream),
            EmailEntityFile.Csv("report.csv", fileStream),
            EmailEntityFile.Stream("report.stream", fileStream),
            EmailEntityFile.Pdf("report.pdf", fileStream),
            EmailEntityFile.Text("report.text", fileStream)
        );

        // Act
        await sender.Send(email);

        // Assert
        var result = _smtpServer.ReceivedEmail;
        result.Any(x => x.FromAddress.Address == from && x.ToAddresses.Any(t => t.Address == to)).ShouldBeTrue();
    }
}