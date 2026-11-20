using NLog;
using Npgsql;

namespace BoardGameTracker.Api.Configuration;

public class PostgresConfguration
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = "board_game_tracker";

    public void Print()
    {
        Logger.Info("Host: {host}", Host);
        Logger.Info("Port: {port}", Port);
        Logger.Info("Username: {username}", Username);
        Logger.Info("Password: {password}", new string('*', Password.Length));
        Logger.Info("Database: {database}", Database);
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
