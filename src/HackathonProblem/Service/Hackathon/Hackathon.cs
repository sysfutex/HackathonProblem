using System.Collections.ObjectModel;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hackathon;

public class Hackathon : IHackathon
{
    private readonly Random _random = new();

    public (IEnumerable<Wishlist> TeamLeadsWishlists, IEnumerable<Wishlist> JuniorsWishlists) Start(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors
    )
    {
        var readOnlyTeamLeads = new ReadOnlyCollection<Employee>(teamLeads.ToList());
        var readOnlyJuniors = new ReadOnlyCollection<Employee>(juniors.ToList());

        return (
            GenerateWishlists(readOnlyTeamLeads, readOnlyJuniors),
            GenerateWishlists(readOnlyJuniors, readOnlyTeamLeads)
        );
    }

    private IEnumerable<Wishlist> GenerateWishlists(IEnumerable<Employee> owners, IEnumerable<Employee> members)
    {
        var wishlists = new List<Wishlist>();
        var readOnlyMembers = new ReadOnlyCollection<Employee>(members.ToList());

        foreach (var owner in owners)
        {
            var preferenceIds = readOnlyMembers.Select(m => m.Id).ToArray();
            var count = preferenceIds.Length;

            while (count > 1)
            {
                --count;

                var rand = _random.Next(count + 1);
                (preferenceIds[rand], preferenceIds[count]) = (preferenceIds[count], preferenceIds[rand]);
            }

            wishlists.Add(new Wishlist(owner.Id, preferenceIds));
        }

        return wishlists.AsEnumerable();
    }
}
