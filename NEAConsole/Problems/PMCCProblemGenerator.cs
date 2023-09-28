using NEAConsole.Statistics;

namespace NEAConsole.Problems;
internal class PMCCProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Product Moment Correlation Coefficient";
    public string SkillPath => "Statistics.PMCC";
    private readonly IRandom random;

    public IProblem Generate(Skill knowledge)
    {
        int n = random.Next(9, 13);
        var data = Enumerable.Range(0, n).Select(n => ((double)random.Next(-10, 10), (double)random.Next(-10, 10))); // WILL GIVE AWFUL R VALUES

        var sealedData = data.ToList();

        return new PMCCProblem(sealedData, new Regression(sealedData).PMCC());
    }

    public PMCCProblemGenerator() : this(new Random()) { }
    public PMCCProblemGenerator(IRandom randomNumberGenerator) => random = randomNumberGenerator;
}