using HackathonProblem.Config;
using HackathonProblem.Exception;
using HackathonProblem.Service.Hackathon;
using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Service.Registrar;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        logger.LogDebug("teamLeads:\n{teamLeads}", string.Join("\n", readOnlyTeamLeads));
        logger.LogDebug("juniors:\n{juniors}", string.Join("\n", readOnlyJuniors));

        logger.LogDebug("_hackathonCount: {_hackathonCount}", _hackathonCount);

        var average = 0.0;
        for (var i = 0; i < _hackathonCount; ++i)
        {
            try
            {
                // Проведение хакатона (составление вишлистов)
                var hackathon = new Hackathon();
                var (teamLeadsWishlists, juniorsWishlists) = hackathon.Start(readOnlyTeamLeads, readOnlyJuniors);
                var (readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists) = (teamLeadsWishlists.ToList().AsReadOnly(), juniorsWishlists.ToList().AsReadOnly());
                logger.LogDebug("teamLeadsWishlists (i: {i}):\n{teamLeadsWishlists}", i, string.Join("\n", readOnlyTeamLeadsWishlists));
                logger.LogDebug("juniorsWishlists (i: {i}):\n{juniorsWishlists}", i, string.Join("\n", readOnlyJuniorsWishlists));

                // Формирование команд
                var teams = hrManager.BuildTeams(readOnlyTeamLeads, readOnlyJuniors, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);
                var readOnlyTeams = teams.ToList().AsReadOnly();
                logger.LogDebug("teams:\n{teams}", string.Join("\n", readOnlyTeams));

                // Подсчет среднего гармонического
                var harmony = hrDirector.CalculateHarmonicMean(readOnlyTeams, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);
                logger.LogInformation("harmony: {harmony}", harmony);

                average += harmony;
            }
            catch (InvalidWishlistException exception)
            {
                logger.LogError($"An error occurred while creating the wishlist. {exception.Message}");
            }
        }

        average /= _hackathonCount;
        logger.LogInformation("average: {average}", average);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
