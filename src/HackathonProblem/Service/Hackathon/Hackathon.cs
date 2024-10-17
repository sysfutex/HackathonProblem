using Nsu.HackathonProblem.Contracts;

namespace HackathonProblem.Service.Hackathon;

public class Hackathon : IHackathon
{
    private readonly Random _random = new();

    public (IEnumerable<Wishlist> TeamLeadsWishlists, IEnumerable<Wishlist> JuniorsWishlists) GenerateWishlists(
        IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors
    )
    {
        var readOnlyTeamLeads = teamLeads.ToList().AsReadOnly();
        var readOnlyJuniors = juniors.ToList().AsReadOnly();

        return (
            GenerateWishlistsFor(readOnlyTeamLeads, readOnlyJuniors),
            GenerateWishlistsFor(readOnlyJuniors, readOnlyTeamLeads)
        );
    }

    private IEnumerable<Wishlist> GenerateWishlistsFor(IEnumerable<Employee> owners, IEnumerable<Employee> members)
    {
        var wishlists = new List<Wishlist>();
        var readOnlyMembers = members.ToList().AsReadOnly();

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
