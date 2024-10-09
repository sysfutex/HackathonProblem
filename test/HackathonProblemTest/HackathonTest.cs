using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Strategy.Marriage;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblemTest;

public class HackathonTest
{
    // Хакатон, в котором заранее определены участники и списки предпочтений, должен дать определённый уровень гармоничности
    [Fact]
    public void TestCertainParticipantsAndPreferences()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий"), new(12, "Климов Михаил") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита"), new(17, "Маркина Кристина") }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16, 17]), new(11, [16, 15, 17]), new(12, [17, 15, 16]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10, 11, 12]), new(16, [11, 10, 12]), new(17, [12, 10, 11]) }.AsReadOnly();

        const decimal expected = 3.0m;

        // ACT
        var teams = new HrManager(new MarriageStrategy()).BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList().AsReadOnly();
        var harmony = new HrDirector().CalculateHarmonicMean(teams, teamLeadsWishlists, juniorsWishlists);

        // ASSERT
        Assert.True(Math.Abs(expected - harmony) < 0.001m);
    }
}
