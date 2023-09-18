using NEAConsole;

namespace NEAConsoleTests;
internal static class TestHelpers
{
    internal static bool GenericTest(IProblemGenerator pg, IAnswer ans, Skill? knowledge = null)
    {
        knowledge ??= Skill.KnowledgeConstructor("SampleKnowledge.json");
        var problem = pg.Generate(knowledge);

        return problem.EvaluateAnswer(ans);
    }
}