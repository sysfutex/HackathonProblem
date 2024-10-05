using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Director;

public interface IHrDirector
{
    double Calc(
        IEnumerable<Team> teams,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
    );
}
