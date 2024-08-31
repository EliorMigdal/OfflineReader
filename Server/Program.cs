using Server.Services;

namespace Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(8080);
        });
        
        builder.Services.AddHostedService<TimedHostedService>();
        builder.Services.AddControllers();
        var app = builder.Build();
        
        app.MapGet("/", () => "Hello World! Welcome to the Offline Reader server!");
        app.MapGet("/health", () => Results.Ok("Offline Reader server is up and running!"));
        app.MapGet("/initDB", () =>
        {
            DBService.Instance.InitializeDatabase();
            Results.Ok("Database initialized successfully!");
        });
        app.MapControllers(); 
        app.Run();
    }
}