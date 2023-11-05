namespace NEAConsole.Problems;
internal class RandomProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Random Questions";
    public string SkillPath => "-"; // special symbol for not a valid ProblemGenerator to randomly select
    private readonly IRandom random;

    public IProblem Generate(Skill knowledge)
    {
        var generators = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.GetInterfaces().Contains(typeof(IProblemGenerator)))
                            .Select(t => (IProblemGenerator)Activator.CreateInstance(t)!)
                            .Where(g => knowledge.Query(g.SkillPath, out _) && g.SkillPath != SkillPath).ToArray();

        return generators[random.Next(generators.Length)].Generate(knowledge);
    }

    public static double GetScore(Skill skill)
        => (double)(skill.TotalCorrect + 1) / (skill.TotalAttempts+5) * (skill.LastRevised - new DateTime(2023, 1, 1)).TotalMinutes;

    public IProblemGenerator GetNextBestPG(IList<(IProblemGenerator pg, Skill s)> skills)
    {
        IProblemGenerator minPG = skills.First().pg;
        double minScore = double.MaxValue;

        foreach (var (pg, s) in skills)
        {
            if (s.TotalAttempts < 5) continue;
            var score = GetScore(s);
            if (score < minScore)
            {
                minPG = pg;
                minScore = score;
            }
        }

        if (minScore == double.MaxValue) return skills[random.Next(0, skills.Count)].pg;
        return minPG;
    }

    public IProblem GenerateNextBest(Skill knowledge)
    {
        var generators = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.GetInterfaces().Contains(typeof(IProblemGenerator)))
                            .Select(t => (IProblemGenerator)Activator.CreateInstance(t)!)
                            .Where(g => knowledge.Query(g.SkillPath, out _) && g.DisplayText != DisplayText).ToArray();

        //                                                                                              not sure if this where is needed    select is just to stop compiler whining
        var skills = generators.Select(g => knowledge.Query(g.SkillPath, out Skill? s) ? (g, s) : (g, s)).Where(t => t.s is not null).Select(t => (t.g, t.s!)).ToList();

        return GetNextBestPG(skills).Generate(knowledge);
    }

    public RandomProblemGenerator() : this(new Random()) { }
    public RandomProblemGenerator(IRandom randomNumberGenerator) => random = randomNumberGenerator;
}