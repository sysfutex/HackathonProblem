using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Director;

public interface IHrDirector
{
    double CalculateHarmonicMean(
        IEnumerable<Team> teams,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
    );
}
