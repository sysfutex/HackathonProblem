using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Manager;

public class HrManager(ITeamBuildingStrategy strategy) : IHrManager
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
    )
    {
        foreach (var teamLeadWishlist in teamLeadsWishlists)
        {
            if (teamLeadWishlist.DesiredEmployees.Length != juniors.Count())
            {
                throw new ArgumentException($"The number of the team lead's preferences does not match the number of juniors (owner ID: {teamLeadWishlist.EmployeeId})");
            }
        }

        foreach (var juniorWishlist in juniorsWishlists)
        {
            if (juniorWishlist.DesiredEmployees.Length != teamLeads.Count())
            {
                throw new ArgumentException($"The number of the junior's preferences does not match the number of team leads (owner ID: {juniorWishlist.EmployeeId})");
            }
        }

        return strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    }
}
