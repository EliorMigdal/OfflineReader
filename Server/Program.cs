using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddHostedService<TimedHostedService>();
        builder.Services.AddControllers();
        var app = builder.Build();
        
        app.MapGet("/", () => "Hello World!");
        app.MapGet("/health", () => Results.Ok("Server is up and running"));
        app.MapControllers();
        app.Run();
    }
}