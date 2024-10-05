using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Manager;

public interface IHrManager
{
    IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
    );
}
