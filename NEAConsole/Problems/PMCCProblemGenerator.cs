using NEAConsole.Statistics;

namespace NEAConsole.Problems;
internal class PMCCProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Product Moment Correlation Coefficient";
    public string SkillPath => "Statistics.PMCC";
    private readonly Random random;

    public IProblem Generate(Skill knowledge)
    {
        int n = random.Next(9, 13);
        var data = Enumerable.Range(0, n).Select(n => ((double)random.Next(-10, 10), (double)random.Next(-10, 10))); // WILL GIVE AWFUL R VALUES

        return new PMCCProblem(data.ToList(), new Regression(data).PMCC());
    }

    public PMCCProblemGenerator() : this(new Random()) { }
    public PMCCProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}