using HackathonProblem.Service.Hackathon;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblemTest;

public class WishlistTest
{
    // Размер списка должен совпадать с количеством тимлидов/джунов
    [Fact]
    public void TestWishlistSize()
    {
        // ARRANGE
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), new(11, "Демидов Дмитрий"), new(12, "Климов Михаил") }.AsReadOnly();
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), new(16, "Фомин Никита"), new(17, "Маркина Кристина") }.AsReadOnly();

        // ACT
        var (teamLeadsWishlists, juniorsWishlists) = new Hackathon().GenerateWishlists(teamLeads, juniors);

        // ASSERT
        foreach (var teamLeadWishlist in teamLeadsWishlists)
        {
            Assert.Equal(teamLeadWishlist.DesiredEmployees.Length, juniors.Count);
        }

        foreach (var juniorWishlist in juniorsWishlists)
        {
            Assert.Equal(juniorWishlist.DesiredEmployees.Length, teamLeads.Count);
        }
    }

    // Заранее определенный сотрудник должен присутствовать в списке
    [Fact]
    public void TestWishlistContent()
    {
        // ARRANGE
        var certainTeamLead = new Employee(11, "Демидов Дмитрий");
        var teamLeads = new List<Employee> { new(10, "Астафьев Андрей"), certainTeamLead, new(12, "Климов Михаил") }.AsReadOnly();

        var certainJunior = new Employee(16, "Фомин Никита");
        var juniors = new List<Employee> { new(15, "Добрынин Степан"), certainJunior, new(17, "Маркина Кристина") }.AsReadOnly();

        // ACT
        var (teamLeadsWishlists, juniorsWishlists) = new Hackathon().GenerateWishlists(teamLeads, juniors);

        // ASSERT
        foreach (var teamLeadWishlist in teamLeadsWishlists)
        {
            Assert.Contains(certainJunior.Id, teamLeadWishlist.DesiredEmployees);
        }

        foreach (var juniorWishlist in juniorsWishlists)
        {
            Assert.Contains(certainTeamLead.Id, juniorWishlist.DesiredEmployees);
        }
    }
}
