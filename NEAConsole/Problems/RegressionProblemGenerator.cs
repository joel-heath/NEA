using NEAConsole.Statistics;

namespace NEAConsole.Problems;

public class RegressionProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "Linear Regression";
    public string SkillPath => "Statistics.Regression";
    private readonly IRandom random = randomNumberGenerator;

    public IProblem Generate(Skill knowledge)
    {
        int n = random.Next(9, 13);
        var data = Enumerable.Range(0, n).Select(n => ((double)random.Next(-10, 10), (double)random.Next(-10, 10))); // WILL GIVE AWFUL R VALUES

        var sealedData = data.ToList();

        return new RegressionProblem(sealedData, new Regression(sealedData).LeastSquaresXonY());
    }

    public RegressionProblemGenerator() : this(new Random()) { }
}