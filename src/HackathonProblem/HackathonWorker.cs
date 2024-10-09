using HackathonProblem.Config;
using HackathonProblem.Exception;
using HackathonProblem.Service.Hackathon;
using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Service.Registrar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;

namespace HackathonProblem;

public class HackathonWorker(
    ILogger<HackathonWorker> logger,
    IOptions<HackathonConfig> hackathonConfigOptions,
    IRegistrar registrar,
    IHrManager hrManager,
    IHrDirector hrDirector
) : IHostedService
{
    private readonly int _hackathonCount = hackathonConfigOptions.Value.HackathonCount;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // "Регистрация" участников хакатона (парсинг csv-файлов)
        var (teamLeads, juniors) = registrar.Register();
        var (readOnlyTeamLeads, readOnlyJuniors) = (teamLeads.ToList().AsReadOnly(), juniors.ToList().AsReadOnly());
        Log.Debug("readOnlyTeamLeads: {readOnlyTeamLeads}", readOnlyTeamLeads);
        Log.Debug("readOnlyJuniors: {readOnlyJuniors}", readOnlyJuniors);

        logger.LogDebug("_hackathonCount: {_hackathonCount}", _hackathonCount);

        var average = 0.0m;
        for (var i = 0; i < _hackathonCount; ++i)
        {
            try
            {
                // Проведение хакатона (составление вишлистов)
                var hackathon = new Hackathon();
                var (teamLeadsWishlists, juniorsWishlists) = hackathon.Start(readOnlyTeamLeads, readOnlyJuniors);
                var (readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists) = (teamLeadsWishlists.ToList().AsReadOnly(), juniorsWishlists.ToList().AsReadOnly());
                Log.Debug("readOnlyTeamLeadsWishlists: {readOnlyTeamLeadsWishlists}", readOnlyTeamLeadsWishlists);
                Log.Debug("readOnlyJuniorsWishlists: {readOnlyJuniorsWishlists}", readOnlyJuniorsWishlists);

                // Формирование команд
                var teams = hrManager.BuildTeams(readOnlyTeamLeads, readOnlyJuniors, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);
                var readOnlyTeams = teams.ToList().AsReadOnly();
                Log.Debug("readOnlyTeams: {readOnlyTeams}", readOnlyTeams);

                // Подсчет среднего гармонического
                var harmony = hrDirector.CalculateHarmonicMean(readOnlyTeams, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);
                Log.Information($"harmony: {harmony}");

                average += harmony;
            }
            catch (InvalidWishlistException exception)
            {
                Log.Error($"An error occurred while creating the wishlist. {exception.Message}");
            }
        }

        average /= _hackathonCount;
        Log.Information($"average: {average}");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
