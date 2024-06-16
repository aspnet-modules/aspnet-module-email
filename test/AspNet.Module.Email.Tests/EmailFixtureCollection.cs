namespace AspNet.Module.Email.Tests;

[CollectionDefinition(Name)]
public class EmailFixtureCollection : ICollectionFixture<EmailTestFixture>
{
    public const string Name = nameof(EmailFixtureCollection);
}