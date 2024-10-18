using HackathonProblem.Service.Hr.Manager;
using HackathonProblem.Strategy.Marriage;
using Moq;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblemTest;

public class HrManagerTest
{
    // При несоответствии количества тимлидов размеру вишлиста джуна выбрасывается исключение
    [Fact]
    public void TestNumberOfTeamLeads()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита") }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16]), new(11, [16, 15]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10]), new(16, [11, 10]) }.AsReadOnly();

        // ACT & ASSERT
        Assert.Throws<ArgumentException>(() => new HrManager(new MarriageStrategy()).BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists));
    }

    // При несоответствии количества джунов размеру вишлиста тимлида выбрасывается исключение
    [Fact]
    public void TestNumberOfJuniors()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита") }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16]), new(11, [16]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10, 11]), new(16, [11, 10]) }.AsReadOnly();

        // ACT & ASSERT
        Assert.Throws<ArgumentException>(() => new HrManager(new MarriageStrategy()).BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists));
    }

    // Количество команд должно совпадать с заранее заданным
    [Fact]
    public void TestNumberOfTeams()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита") }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16]), new(11, [16, 15]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10, 11]), new(16, [11, 10]) }.AsReadOnly();

        const int expected = 2;

        // ACT
        var teams = new HrManager(new MarriageStrategy()).BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists).ToList().AsReadOnly();

        // ASSERT
        Assert.Equal(expected, teams.Count);
    }

    // Стратегия HR менеджера должна быть вызвана ровно один раз
    [Fact]
    public void TestNumberOfStrategyCalls()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита") }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16]), new(11, [16, 15]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10, 11]), new(16, [11, 10]) }.AsReadOnly();

        var mockStrategy = new Mock<ITeamBuildingStrategy>();
        var hrManager = new HrManager(mockStrategy.Object);

        // ACT
        hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

        // ASSERT
        mockStrategy.Verify(d => d.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists), Times.Once);
    }
}
