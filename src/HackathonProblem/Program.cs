using HackathonProblem.Config;
using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Service.Hr.Manager.Strategy;
using HackathonProblem.Service.Registrar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem;

public static class Program
{
    public static async Task Main(string[] args)
    {
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
                services.AddSingleton<ITeamBuildingStrategy, Marriage>();
                services.AddSingleton<IHrManager, HrManager>();
                services.AddSingleton<IHrDirector, HrDirector>();
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "dd.MM.yyyy HH:mm:ss:fff ";
                    options.UseUtcTimestamp = true;
                });
                logging.SetMinimumLevel(LogLevel.Information);
            }).Build();

        await host.StartAsync();
        await host.StopAsync();
    }
}
