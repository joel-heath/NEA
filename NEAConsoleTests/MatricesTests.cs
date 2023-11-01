using NEAConsole;
using NEAConsole.Problems;
using NEAConsole.Matrices;

namespace NEAConsoleTests;

public class MatricesTests
{
    private static FakeRandom MatrixAdditionDataStreamGenerator(char mode, Matrix mat1, Matrix mat2)
        => new(new int[] { mode == '+' ? 0 : 1, mat1.Rows, mat1.Columns }.Concat(mat1.Select(i => (int)i)).Concat(mat2.Select(i => (int)i)));

    private static FakeRandom MatrixMultiplicationDataStreamGenerator(Matrix mat1, Matrix mat2)
        => new(new int[] { mat1.Rows, mat1.Columns }.Concat(mat1.Select(i => (int)i)).Append(mat2.Columns).Concat(mat1.Select(i => (int)i)));

    private static FakeRandom MatrixDeterminantDataStreamGenerator(Matrix mat)
        => new(new int[] { mat.Rows, mat.Columns }.Concat(mat.Select(i => (int)i)));

    [Test]
    public void AdditionTest()
    {
        char mode = '+';
        var mat1 = new Matrix(new double[,] { { 8 }, { -4 }, { 8 } });
        var mat2 = new Matrix(new double[,] { { 6 }, { 0 }, { 2 } });
        var mat3 = new Matrix(new double[,] { { 14 }, { -4 }, { 10 } });

        Assert.That(TestHelpers.GenericTest(new MatricesAdditionProblemGenerator(MatrixAdditionDataStreamGenerator(mode, mat1, mat2)), new MatrixAnswer(mat3)));
        Assert.Pass();
    }

    [Test]
    public void MultiplicationTest()
    {
        var mat1 = new Matrix(new double[,]
        {
            { -4, 8, -4 },
            { 8, 6, 0 }
        });
        var mat2 = new Matrix(new double[,]
        {
            { -4, -2 },
            { 7, 8 },
            { -1, 4 }
        });
        var mat3 = new Matrix(new double[,]
        {
            { 76, 56 },
            { 10, 32 }
        });

        Assert.That(TestHelpers.GenericTest(new MatricesAdditionProblemGenerator(MatrixMultiplicationDataStreamGenerator(mat1, mat2)), new MatrixAnswer(mat3)));
        Assert.Pass();
    }

    [Test]
    public void DeterminantTest()
    {
        var mat = new Matrix(new double[,]
        {
            { 7, -4 },
            { 8, -4 }
        });
        var answer = 4;

        Assert.That(TestHelpers.GenericTest(new MatricesDeterminantsProblemGenerator(MatrixDeterminantDataStreamGenerator(mat)), new IntAnswer(answer)));
        Assert.Pass();
    }
}