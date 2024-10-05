using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hackathon;

public interface IHackathon
{
    (IEnumerable<Wishlist> TeamLeadsWishlists, IEnumerable<Wishlist> JuniorsWishlists) Start(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors
    );
}
