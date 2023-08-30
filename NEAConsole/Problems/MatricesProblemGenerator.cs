namespace NEAConsole.Problems;
internal class MatricesProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrices";
    public string SkillPath => string.Empty;
    private readonly Random random;

    public IProblem Generate()
    {
        var generators = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.GetInterfaces().Contains(typeof(IProblemGenerator)))
                        .Select(t => (IProblemGenerator)Activator.CreateInstance(t)!)
                        .Where(g => Program.Knowledge.IsKnown(g.SkillPath) && g.SkillPath.Contains("Matrices")).ToArray();

        // weighted random here?
        return generators[random.Next(generators.Length)].Generate();
    }

    public MatricesProblemGenerator() : this(new Random()) { }
    public MatricesProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}