namespace NEAConsole.Problems;
internal class MatricesProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrices";
    public string SkillPath => string.Empty;
    private readonly Random random;

    public IProblem Generate(Skill knowledge)
    {
        // currently this is always true              this may not necessarily be true
        while (!knowledge.Known || !knowledge.Children.First(s => s.Name == DisplayText).Known)
        {
            Console.WriteLine("To use random questions, you must first enter the topics you know.");
            UIMethods.Wait(string.Empty);
            Console.Clear();
            Program.UpdateKnowledge(knowledge);
        }

        var generators = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                        .Where(t => t.GetInterfaces().Contains(typeof(IProblemGenerator)))
                        .Select(t => (IProblemGenerator)Activator.CreateInstance(t)!)
                        .Where(g => knowledge.Query(g.SkillPath, out _) && g.SkillPath.Contains("Matrices")).ToArray();

        // weighted random here?
        return generators[random.Next(generators.Length)].Generate(knowledge);
    }

    public MatricesProblemGenerator() : this(new Random()) { }
    public MatricesProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}