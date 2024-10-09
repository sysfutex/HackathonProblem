namespace HackathonProblem.Service.Hr.Director.Calculator;

public static class HarmonicMeanCalculator
{
    public static decimal Calculate(IEnumerable<int> satisfactions)
    {
        var readOnlySatisfactions = satisfactions.ToList().AsReadOnly();

        return readOnlySatisfactions.Count / readOnlySatisfactions.Sum(satisfaction => 1.0m / satisfaction);
    }
}
