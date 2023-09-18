using NEAConsole;
using NEAConsole.Problems;
using NEAConsole.Matrices;

namespace NEAConsoleTests;
public class MatricesTests
{
    [Test]
    public void AdditionTest()
    {
        Assert.That(TestHelpers.GenericTest(new MatricesAdditionProblemGenerator(new(1234)), new MatrixAnswer(new Matrix(new double[,] { { 14 }, { -4 }, { 10 } }))));
        Assert.Pass();

        // [8 ] + [6]
        // [-4] + [0]
        // [8 ] + [2]
    }

    [Test]
    public void MultiplicationTest()
    {
        Assert.That(TestHelpers.GenericTest(new MatricesMultiplicationProblemGenerator(new(1234)), new MatrixAnswer(new Matrix(new double[,] { { 76, 56 }, { 10, 32 } }))));
        Assert.Pass();

        // [-4 8 -4]   [-4 -2]
        // [8  6 0 ] * [7  8 ]
        //             [-1 4 ]
    }

    [Test]
    public void DeterminantTest()
    {
        Assert.That(TestHelpers.GenericTest(new MatricesDeterminantsProblemGenerator(new(1234)), new IntAnswer(4)));
        Assert.Pass();

        // [7 -4]
        // [8 -4]
    }

    [Test]
    public void InversionTest2x2()
    {
        Assert.That(TestHelpers.GenericTest(new MatricesInversionProblemGenerator(new(1234)), new MatrixAnswer(new Matrix(new double[,] { { 2, -3 }, { -5, 8 } }))));
        Assert.Pass();

        // [8 3]
        // [5 2]
    }
}


    /*
     var pg = new ProblemGenerator(new(1234));
     var kn = Skill.KnowledgeConstructor("SampleKnowledge.json");
     var p = pg.Generate(kn);
    */