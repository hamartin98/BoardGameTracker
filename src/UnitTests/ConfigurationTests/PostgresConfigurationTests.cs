using BoardGameTracker.Api.Configuration;
using Npgsql;

namespace BoardGameTracker.UnitTests.ConfigurationTests;

public class PostgresConfigurationTests
{
    [Theory]
    [InlineData("simple")]
    [InlineData("with;semicolon")]
    [InlineData("with=equals")]
    [InlineData("with'quote\"double")]
    [InlineData(" padded ")]
    public void PasswordRoundTripsThroughConnectionStringTest(string password)
    {
        var config = new PostgresConfguration { Password = password };

        var parsed = new NpgsqlConnectionStringBuilder(config.GetConnectionString());

        Assert.Equal(password, parsed.Password);
        Assert.Equal(config.Host, parsed.Host);
        Assert.Equal(config.Port, parsed.Port);
        Assert.Equal(config.Username, parsed.Username);
        Assert.Equal(config.Database, parsed.Database);
    }
}
