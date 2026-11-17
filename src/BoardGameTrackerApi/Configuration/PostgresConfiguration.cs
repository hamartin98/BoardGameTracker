namespace BoardGameTracker.Api.Configuration;

public class PostgresConfguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5432;
    public string Username { get; set; } = "postgres";
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = "board_game_tracker";

    public void Print()
    {
        Console.WriteLine($"Host: {Host}");
        Console.WriteLine($"Port: {Port}");
        Console.WriteLine($"Username: {Username}");
        Console.WriteLine($"Password: {'*' * Password.Length}");
        Console.WriteLine($"Database: {Database}");
    }

    public string GetConnectionString()
    {
        return $"Host={Host};Port={Port};Password={Password};Username={Username};Database={Database}";
    }
}
