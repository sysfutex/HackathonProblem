using HackathonProblem.Config;
using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Service.Registrar;
using HackathonProblem.Strategy.Marriage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.HackathonProblem.Contracts;
using Serilog;

namespace HackathonProblem;

public static class Program
{
    public static async Task Main(string[] args)
    {
        ConfigureLogger();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                // Добавление конфигурации из файла appsettings.json
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Регистрация настроек из appsettings.json
                services.Configure<EmployeeLists>(context.Configuration.GetSection("EmployeeLists"));
                services.Configure<HackathonConfig>(context.Configuration.GetSection("HackathonConfig"));

                services.AddHostedService<HackathonWorker>();

                // Регистрация зависимостей
                services.AddSingleton<IRegistrar, Registrar>();
                services.AddSingleton<ITeamBuildingStrategy, MarriageStrategy>();
                services.AddSingleton<IHrManager, HrManager>();
                services.AddSingleton<IHrDirector, HrDirector>();
            }).Build();

        await host.StartAsync();
        await host.StopAsync();
    }

    private static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Level:u3} {Timestamp:dd.MM.yyyy HH:mm:ss:fff}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
