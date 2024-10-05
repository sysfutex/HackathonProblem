using HackathonProblem.Config;
using HackathonProblem.Service.Hackathon;
using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Service.Hr.Manager.Strategy;
using HackathonProblem.Service.Registrar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        var log = host.Services.GetRequiredService<ILogger<Program>>();
        
        // "Регистрация" участников хакатона (парсинг csv-файлов)
        var registrar = host.Services.GetRequiredService<IRegistrar>();
        var (teamLeads, juniors) = registrar.Register();
        var (teamLeadsList, juniorsList) = (teamLeads.ToList(), juniors.ToList());
        log.LogDebug("teamLeads:\n{teamLeads}", string.Join("\n", teamLeadsList));
        log.LogDebug("juniors:\n{juniors}", string.Join("\n", juniorsList));
        
        var hackathonCount = host.Services.GetRequiredService<IOptions<HackathonConfig>>().Value.HackathonCount;
        log.LogDebug("hackathonCount: {hackathonCount}", hackathonCount);
        
        var average = 0.0;
        for (var i = 0; i < hackathonCount; ++i)
        {
            // Проведение хакатона (составление вишистов)
            var hackathon = host.Services.GetRequiredService<IHackathon>();
            var (teamLeadsWishlists, juniorsWishlists) = hackathon.Start(teamLeadsList, juniorsList);
            log.LogDebug("teamLeadsWishlists (i: {i}):\n{teamLeadsWishlists}", i, string.Join("\n", teamLeadsWishlists));
            log.LogDebug("juniorsWishlists (i: {i}):\n{juniorsWishlists}", i, string.Join("\n", juniorsWishlists));
        
            // Формирование команд
            var hrManager = host.Services.GetRequiredService<IHrManager>();
            var teams = hrManager.BuildTeams(teamLeadsList, juniorsList, teamLeadsWishlists, juniorsWishlists);
            log.LogDebug("teams:\n{teams}", string.Join("\n", teams));
        
            // Подсчет среднего гармонического
            var hrDirector = host.Services.GetRequiredService<IHrDirector>();
            var harmony = hrDirector.Calc(teams, teamLeadsWishlists, juniorsWishlists);
            log.LogInformation("harmony: {harmony}", harmony);
        
            average += harmony;
        }
        
        average /= hackathonCount;
        log.LogInformation("average: {average}", average);
    }
    
    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
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

                // Регистрация зависимостей
                services.AddSingleton<IRegistrar, Registrar>();
                services.AddTransient<IHackathon, Hackathon>();
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
            });
}
