using NEAConsole;
using NEAConsole.Problems;
using Random = NEAConsole.Random;

namespace NEAConsoleTests;

public class GraphsTests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void DijkstrasTest()
    {
        Assert.That(TestHelpers.GenericTest(new DijkstrasProblemGenerator(new Random(1234)), new IntAnswer(26)));
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
        Assert.That(TestHelpers.GenericTest(new PrimsProblemGenerator(new Random(1234)), new PrimsAnswer([(1, 0), (2, 1), (5, 1), (8, 5), (4, 2), (3, 2), (7, 3), (6, 5)])));
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