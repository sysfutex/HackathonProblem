namespace HackathonProblem.Service.Hr.Director.Calculator;

public class HarmonicMeanCalculator : IHarmonyCalculator
{
    public decimal CalculateHarmony(IEnumerable<int> satisfactions)
    {
        var readOnlySatisfactions = satisfactions.ToList().AsReadOnly();

        return readOnlySatisfactions.Count / readOnlySatisfactions.Sum(satisfaction => 1.0m / satisfaction);
    }
}
