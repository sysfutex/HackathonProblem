using HackathonProblem.Service.Hackathon;
using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Strategy.Marriage;
using Moq;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblemTest;

public class HrManagerTest
{
    // Количество команд должно совпадать с заранее заданным
    [Fact]
    public void TestNumberOfTeams()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита") }.AsReadOnly();

        var (teamLeadsWishlists, juniorsWishlists) = new Hackathon().Start(teamLeads, juniors);

        // ACT
        var teams = new HrManager(new MarriageStrategy()).BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList().AsReadOnly();

        // ASSERT
        Assert.Equal(teams.Count, teamLeads.Count);
        Assert.Equal(teams.Count, juniors.Count);
    }

    // Стратегия HR менеджера должна быть вызвана ровно один раз
    [Fact]
    public void TestNumberOfStrategyCalls()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита") }.AsReadOnly();

        var (teamLeadsWishlists, juniorsWishlists) = new Hackathon().Start(teamLeads, juniors);
        var (readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists) = (teamLeadsWishlists.ToList().AsReadOnly(), juniorsWishlists.ToList().AsReadOnly());

        var mockStrategy = new Mock<ITeamBuildingStrategy>();
        var hrManager = new HrManager(mockStrategy.Object);

        // ACT
        hrManager.BuildTeams(teamLeads, juniors, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists);

        // ASSERT
        mockStrategy.Verify(d => d.BuildTeams(teamLeads, juniors, readOnlyTeamLeadsWishlists, readOnlyJuniorsWishlists), Times.Once);
    }
}
