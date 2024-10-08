using HackathonProblem.Config;
using HackathonProblem.Exception;
using HackathonProblem.Service.Hackathon;
using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Service.Hr.Manager.Strategy;
using HackathonProblem.Service.Registrar;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Nsu.HackathonProblem.Contracts;
using Serilog;

namespace HackathonProblem;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        ConfigureLogger();

        // "Регистрация" участников хакатона (парсинг csv-файлов)
        var registrar = host.Services.GetRequiredService<IRegistrar>();
        var (teamLeads, juniors) = registrar.Register();
        var (readOnlyTeamLeads, readOnlyJuniors) = (teamLeads.ToList().AsReadOnly(), juniors.ToList().AsReadOnly());
        Log.Debug("readOnlyTeamLeads: {readOnlyTeamLeads}", readOnlyTeamLeads);
        Log.Debug("readOnlyJuniors: {readOnlyJuniors}", readOnlyJuniors);

        var hackathonCount = host.Services.GetRequiredService<IOptions<HackathonConfig>>().Value.HackathonCount;
        Log.Debug("hackathonCount: {hackathonCount}", hackathonCount);

        var average = 0.0;
        for (var i = 0; i < hackathonCount; ++i)
        {
            try
            {
                // Проведение хакатона (составление вишистов)
                var hackathon = host.Services.GetRequiredService<IHackathon>();
                var (teamLeadsWishlists, juniorsWishlists) = hackathon.Start(readOnlyTeamLeads, readOnlyJuniors);
                var (readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists) = (teamLeadsWishlists.ToList().AsReadOnly(), juniorsWishlists.ToList().AsReadOnly());
                Log.Debug("readOnlyTeamLeadsWishlists: {readOnlyTeamLeadsWishlists}", readOnlyTeamLeadsWishlists);
                Log.Debug("readOnlyJuniorsWishlists: {readOnlyJuniorsWishlists}", readOnlyJuniorsWishlists);

                // Формирование команд
                var hrManager = host.Services.GetRequiredService<IHrManager>();
                var teams = hrManager.BuildTeams(readOnlyTeamLeads, readOnlyJuniors, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);
                var readOnlyTeams = teams.ToList().AsReadOnly();
                Log.Debug("readOnlyTeams: {readOnlyTeams}", readOnlyTeams);

                // Подсчет среднего гармонического
                var hrDirector = host.Services.GetRequiredService<IHrDirector>();
                var harmony = hrDirector.CalculateHarmonicMean(readOnlyTeams, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);
                Log.Information($"harmony: {harmony}");

                average += harmony;
            }
            catch (InvalidWishlistException exception)
            {
                Log.Error($"An error occurred while creating the wishlist. {exception.Message}");
            }
        }

        average /= hackathonCount;
        Log.Information($"average: {average}");
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
            });

    private static void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Level:u3} {Timestamp:dd.MM.yyyy HH:mm:ss:fff}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}
