using NLog;
using Npgsql;

namespace BoardGameTracker.Api.Configuration;

public class PostgresConfiguration
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = "board_game_tracker";

    /// <summary>
    /// Whether pending migrations are applied on startup; when false, the app does not start while any are pending
    /// </summary>
    public bool ApplyMigrationsOnStartup { get; set; } = true;

    public void Print()
    {
        Logger.Info("Host: {host}", Host);
        Logger.Info("Port: {port}", Port);
        Logger.Info("Username: {username}", Username);
        Logger.Info("Password: {password}", new string('*', Password.Length));
        Logger.Info("Database: {database}", Database);
        Logger.Info("Apply migrations on startup: {apply}", ApplyMigrationsOnStartup);
    }

    public string GetConnectionString()
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = Host,
            Port = Port,
            Username = Username,
            Password = Password,
            Database = Database,
        }.ConnectionString;
    }
}
