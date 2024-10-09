namespace HackathonProblem.Service.Hr.Director.Calculator;

public static class HarmonicMeanCalculator
{
    public static double Calculate(IEnumerable<int> satisfactions)
    {
        var readOnlySatisfactions = satisfactions.ToList().AsReadOnly();
        
        return readOnlySatisfactions.Count / readOnlySatisfactions.Sum(satisfaction => 1.0 / satisfaction);
    }
}
