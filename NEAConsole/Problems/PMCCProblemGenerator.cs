using NEAConsole.Statistics;

namespace NEAConsole.Problems;

public class PMCCProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "Product Moment Correlation Coefficient";
    public string SkillPath => "Statistics.PMCC";
    private readonly IRandom random = randomNumberGenerator;

    public IProblem Generate(Skill knowledge)
    {
        int n = random.Next(9, 13);
        var data = Enumerable.Range(0, n).Select(n => ((double)random.Next(-10, 10), (double)random.Next(-10, 10))); // WILL GIVE AWFUL R VALUES

        var sealedData = data.ToList();

        return new PMCCProblem(sealedData, new Regression(sealedData).PMCC());
    }

    public static IEnumerable<(double, double)> GenerateData(IRandom random, int n)
    {
        int m = random.NextNotZero(-10, 13), c = random.NextNotZero(-5, 6), leniency = random.Next(2, 4);

        for (int i = 0; i < n; i++)
        {
            int x = random.Next(-10, 13);
            int y = m * i + c;
            int offset = (int)Math.Round(random.NextDouble() * random.Next(1, leniency));

            y += (random.NextDouble() < 0.5 ? -1 : 1) * offset;

            yield return (x, y);
        }
    }

    public PMCCProblemGenerator() : this(new Random()) { }
}