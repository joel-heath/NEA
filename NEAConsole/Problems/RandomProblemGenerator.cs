namespace NEAConsole.Problems;

public class RandomProblemGenerator(Skill knowledge, IRandom randomNumberGenerator)
{
    private readonly IRandom random = randomNumberGenerator;
    private readonly Skill knowledge = knowledge;
    private readonly IReadOnlyList<IProblemGenerator> problemGenerators = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                            .Where(t => t.GetInterfaces().Contains(typeof(IProblemGenerator)))
                            .Select(t => (IProblemGenerator)Activator.CreateInstance(t)!)
                            .Where(g => knowledge.Query(g.SkillPath, out _)).ToList();

    private static double GetScore(Skill skill)
       => (double)(skill.TotalCorrect + 1) / (skill.TotalAttempts + 5) * (skill.LastRevised - new DateTime(2023, 1, 1)).TotalMinutes * skill.Weight;

    // public for unit testing
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

    public (IProblem problem, string skillPath) GenerateNextBest()
    {
        //                                                                                              not sure if this where is needed    select is just to stop compiler whining
        var skills = problemGenerators.Select(g => knowledge.Query(g.SkillPath, out Skill? s) ? (g, s) : (g, s)).Where(t => t.s is not null).Select(t => (t.g, t.s!)).ToList();
        var gen = GetNextBestPG(skills);

        return (GetNextBestPG(skills).Generate(knowledge), gen.SkillPath);
    }

    public IProblem Generate() => problemGenerators[random.Next(problemGenerators.Count)].Generate(knowledge);

    public RandomProblemGenerator(Skill knowledge) : this(knowledge, new Random()) { }
}