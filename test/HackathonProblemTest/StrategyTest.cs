using HackathonProblem.Strategy.Marriage;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblemTest;

public class StrategyTest
{
    // На заранее определённых предпочтениях стратегия должна возвращать заранее определённое распределение
    [Fact]
    public void TestCertainPreferences()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий"), new(12, "Климов Михаил") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита"), new(17, "Маркина Кристина") }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16, 17]), new(11, [16, 15, 17]), new(12, [17, 15, 16]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10, 11, 12]), new(16, [11, 10, 12]), new(17, [12, 10, 11]) }.AsReadOnly();

        var strategy = new MarriageStrategy();

        // ACT
        var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList().AsReadOnly();

        // ASSERT
        Assert.Equal(teams.Count, teamLeads.Count);
        Assert.Equal(teams.Count, juniors.Count);

        for (var i = 0; i < teams.Count; ++i)
        {
            Assert.Contains(new Team(teamLeads[i], juniors[i]), teams);
        }
    }
}
