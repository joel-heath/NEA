

using NEAConsole;
using NEAConsole.Problems;

namespace NEAConsoleTests
{
    public class Tests
    {
        [SetUp]
        public void Setup() { }

        private static bool GenericTest(IProblemGenerator pg, IAnswer ans, Skill? knowledge = null)
        {
            knowledge ??= Skill.KnowledgeConstructor("SampleKnowledge.json");
            var problem = pg.Generate(knowledge);

            return problem.EvaluateAnswer(ans);
        }

        [Test]
        public void DijkstrasTest()
        {
            Assert.That(GenericTest(new DijkstrasProblemGenerator(new(1234)), new IntAnswer(26)));
            Assert.Pass();

            // [0 4  0  0  0  0  0  0  0 ]
            // [4 0  5  0  17 0  0  14 0 ]
            // [0 5  0  10 8  6  0  0  0 ]
            // [0 0  10 0  0  0  0  0  18]
            // [0 17 8  0  0  0  0  0  16]
            // [0 0  6  0  0  0  12 9  0 ]
            // [0 0  0  0  0  12 0  0  0 ]
            // [0 14 0  0  0  9  0  0  8 ]
            // [0 0  0  18 16 0  0  8  0 ]
        }

        [Test]
        public void PrimsTest()
        {
            Assert.That(GenericTest(new PrimsProblemGenerator(new(1234)), new PrimsAnswer(new() { (1, 0), (2, 1), (5, 1), (8, 5), (4, 2), (3, 2), (7, 3), (6, 5) })));
            Assert.Pass();

            // [9 3  0 0  0  0  0 0  0 ]
            // [3 0  3 16 19 4  0 12 0 ]
            // [0 3  0 7  6  0  0 0  0 ]
            // [0 16 7 0  0  0  0 6  17]
            // [0 19 6 0  19 0  0 0  0 ]
            // [0 4  0 0  0  0  8 14 5 ]
            // [0 0  0 0  0  8  0 0  0 ]
            // [0 12 0 6  0  14 0 0  0 ]
            // [0 0  0 17 0  5  0 0  0 ]
        }
    }
}