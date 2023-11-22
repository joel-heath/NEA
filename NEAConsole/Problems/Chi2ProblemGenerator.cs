using NEAConsole.Statistics;

namespace NEAConsole.Problems;

public class Chi2ProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "Chi-Squared Statistic";
    public string SkillPath => "Statistics.Chi2";
    private readonly IRandom random = randomNumberGenerator;

    public IProblem Generate(Skill knowledge)
    {
        int n = random.Next(2, 4);

        var data = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                data[i, j] = random.Next(2, 20);
            }
        }

        return new Chi2Problem(data, ContingencyTables.CalculateChiSquared(data));
    }

    public Chi2ProblemGenerator() : this(new Random()) { }
}