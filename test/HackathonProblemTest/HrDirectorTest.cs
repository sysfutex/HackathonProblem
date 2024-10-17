using HackathonProblem.Service.Hr.Director;
using HackathonProblem.Service.Hr.Director.Calculator;
using Nsu.HackathonProblem.Contracts;

namespace HackathonProblemTest;

public class HrDirectorTest
{
    // Проверка алгоритма вычисления среднего гармонического
    // Например, среднее гармоническое одинаковых чисел равно им всем 
    [Fact]
    public void TestHarmonicMeanAlgorithm()
    {
        // ARRANGE
        const int number = 5;
        var similarNumbers = new List<int> { number, number, number, number, number }.AsReadOnly();

        // ACT
        var harmonicMean = new HarmonicMeanCalculator().CalculateHarmony(similarNumbers);

        // ASSERT
        Assert.Equal(number, harmonicMean);
    }

    // Проверка вычисления среднего гармонического, конкретные примеры
    // Например, 2 и 6 должны дать 3
    [Fact]
    public void TestHarmonicMeanCalculation()
    {
        // ARRANGE
        var numbers = new List<int> { 2, 6 }.AsReadOnly();
        const decimal expected = 3.0m;

        // ACT
        var harmonicMean = new HarmonicMeanCalculator().CalculateHarmony(numbers);

        // ASSERT
        Assert.True(Math.Abs(expected - harmonicMean) < 0.001m);
    }

    // Заранее определённые списки предпочтений и команды должны дать заранее определённое значение
    [Fact]
    public void TestCertainPreferencesAndTeams()
    {
        // ARRANGE
        var teams = new List<Team>
        {
            new(new Employee(10, "Астафьев Андрей"), new Employee(15, "Добрынин Степан")),
            new(new Employee(11, "Демидов Дмитрий"), new Employee(16, "Фомин Никита")),
            new(new Employee(12, "Климов Михаил"), new Employee(17, "Маркина Кристина"))
        }.AsReadOnly();

        var teamLeadsWishlists = new List<Wishlist> { new(10, [15, 16, 17]), new(11, [16, 15, 17]), new(12, [17, 15, 16]) }.AsReadOnly();
        var juniorsWishlists = new List<Wishlist> { new(15, [10, 11, 12]), new(16, [11, 10, 12]), new(17, [12, 10, 11]) }.AsReadOnly();

        const decimal expected = 3.0m;

        // ACT
        var harmony = new HrDirector(new HarmonicMeanCalculator()).CalculateHarmonicMean(teams, teamLeadsWishlists, juniorsWishlists);

        // ASSERT
        Assert.True(Math.Abs(expected - harmony) < 0.001m);
    }
}
