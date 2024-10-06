using HackathonProblem.Exceptions;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hr.Manager.Strategy;

public class Marriage : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists
    )
    {
        // Задача об устойчивом паросочетании, алгоритм Гейла-Шепли
        // https://neerc.ifmo.ru/wiki/index.php?title=Задача_об_устойчивом_паросочетании

        var teams = new List<Team>();
        var freeTeamLeads = new List<Employee>(teamLeads);
        var teamLeadsPreferences = GetTeamLeadsPreferences(teamLeadsWishlists);

        var readOnlyJuniors = juniors.ToList().AsReadOnly();
        var readOnlyJuniorsWishlists = juniorsWishlists.ToList().AsReadOnly();

        // Пока существует свободный тимлид
        while (freeTeamLeads.Count > 0)
        {
            var freeTeamLead = freeTeamLeads.First();
            var freeTeamLeadWishlist = teamLeadsPreferences[freeTeamLead.Id];
            var mostPreferredJuniorId = freeTeamLeadWishlist.First();
            var mostPreferredJunior = readOnlyJuniors.First(junior => junior.Id == mostPreferredJuniorId);

            if (IsJuniorFree(teams, mostPreferredJuniorId))
            {
                // Если джун свободен, то помечаем freeTeamLead и mostPreferredJunior парой
                teams.Add(new Team(freeTeamLead, mostPreferredJunior));
                freeTeamLeads.Remove(freeTeamLead);
            }
            else
            {
                var mostPreferredJuniorWishlist = readOnlyJuniorsWishlists.First(w => w.EmployeeId == mostPreferredJuniorId);
                var currentPartner = teams.First(t => t.Junior.Id == mostPreferredJuniorId).TeamLead;

                var freeTeamLeadPosition = GetPosition(mostPreferredJuniorWishlist, freeTeamLead.Id);
                var currentPartnerPosition = GetPosition(mostPreferredJuniorWishlist, currentPartner.Id);

                // Если mostPreferredJunior больше предпочитает freeTeamLead, чем текущего currentPartner
                if (freeTeamLeadPosition < currentPartnerPosition)
                {
                    // Помечаем freeTeamLead и mostPreferredJunior парой
                    RemoveTeam(teams, currentPartner);
                    teams.Add(new Team(freeTeamLead, mostPreferredJunior));
                    freeTeamLeads.Remove(freeTeamLead);

                    // Вычеркиваем mostPreferredJunior из списка предпочтений currentPartner
                    var currentPartnerWishlist = teamLeadsPreferences[currentPartner.Id];
                    currentPartnerWishlist.Remove(mostPreferredJuniorId);

                    // Помечаем currentPartner свободным
                    freeTeamLeads.Add(currentPartner);
                }
                else
                {
                    // Вычеркиваем mostPreferredJunior из списка предпочтений freeTeamLead
                    freeTeamLeadWishlist.Remove(mostPreferredJuniorId);
                }
            }
        }

        return teams.AsEnumerable();
    }

    private Dictionary<int, List<int>> GetTeamLeadsPreferences(IEnumerable<Wishlist> teamLeadsWishlists)
    {
        var teamLeadsPreferences = new Dictionary<int, List<int>>();

        foreach (var teamLeadWishlist in teamLeadsWishlists)
        {
            teamLeadsPreferences.Add(teamLeadWishlist.EmployeeId, new List<int>(teamLeadWishlist.DesiredEmployees));
        }

        return teamLeadsPreferences;
    }

    private bool IsJuniorFree(IEnumerable<Team> teams, int juniorId) => teams.All(team => team.Junior.Id != juniorId);

    private int GetPosition(Wishlist wishlist, int id)
    {
        for (var i = 0; i < wishlist.DesiredEmployees.Length; ++i)
        {
            if (wishlist.DesiredEmployees[i] == id)
            {
                return i;
            }
        }

        throw new InvalidWishlistException(wishlist, id);
    }

    private void RemoveTeam(List<Team> teams, Employee teamLead)
    {
        for (var i = 0; i < teams.Count; ++i)
        {
            if (teams[i].TeamLead == teamLead)
            {
                teams.RemoveAt(i);
                return;
            }
        }
    }
}
