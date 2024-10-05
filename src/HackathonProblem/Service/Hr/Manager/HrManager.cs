using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Manager;

public class HrManager(ITeamBuildingStrategy strategy) : IHrManager
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors, IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists
    )
    {
        return strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}
