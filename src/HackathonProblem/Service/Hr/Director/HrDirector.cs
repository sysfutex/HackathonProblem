using HackathonProblem.Exception;
using HackathonProblem.Service.Hr.Director.Calculator;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Director;

public class HrDirector : IHrDirector
{
    public decimal CalculateHarmonicMean(
        IEnumerable<Team> teams,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
    )
    {
        var readOnlyTeams = teams.ToList().AsReadOnly();
        var readOnlyTeamLeadsWishlists = teamLeadsWishlists.ToList().AsReadOnly();
        var readOnlyJuniorsWishlists = juniorsWishlists.ToList().AsReadOnly();

        var teamsCount = readOnlyTeams.Count;

        var satisfactions = new List<int>();
        foreach (var team in readOnlyTeams)
        {
            // Индекс удовлетворенности тимлида
            var teamLeadWishlist = readOnlyTeamLeadsWishlists.First(w => w.EmployeeId == team.TeamLead.Id);
            var juniorPosition = GetPosition(teamLeadWishlist, team.Junior.Id);
            satisfactions.Add(teamsCount - juniorPosition);

            // Индекс удовлетворенности джуна
            var juniorWishlist = readOnlyJuniorsWishlists.First(w => w.EmployeeId == team.Junior.Id);
            var teamLeadPosition = GetPosition(juniorWishlist, team.TeamLead.Id);
            satisfactions.Add(teamsCount - teamLeadPosition);
        }

        return HarmonicMeanCalculator.Calculate(satisfactions.AsEnumerable());
    }

    private int GetPosition(Wishlist wishlist, int employeeId)
    {
        for (var i = 0; i < wishlist.DesiredEmployees.Length; ++i)
        {
            if (wishlist.DesiredEmployees[i] == employeeId)
            {
                return i;
            }
        }

        throw new InvalidWishlistException(wishlist, employeeId);
    }
}
