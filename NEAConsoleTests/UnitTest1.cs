

using NEAConsole;
using NEAConsole.Problems;

namespace NEAConsoleTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {

            var rand = new Random(1234);
            var generator = new PrimsProblemGenerator(rand);
            var knowledge = Skill.KnowledgeConstructor("SampleKnowledge.json");
            var problem = generator.Generate(knowledge);

            Assert.That(problem, Is.Not.Null);
            //Assert.That(problem.EvaluateAnswer("1234"), Is.True);

            Assert.Pass();
        }
    }
}