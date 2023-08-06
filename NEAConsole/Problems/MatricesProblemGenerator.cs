using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrices";
    private readonly Random random;

    public IProblem Generate()
    {
        var mode = random.Next(0, 3);
        (int rows, int cols) = (random.Next(1, 4), random.Next(1, 4));

        Matrix mat1 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)random.Next(-10, 10)));
        Matrix mat2 = mode == 2 ? new(cols, rows = random.Next(0, 4), Enumerable.Range(0, rows * cols).Select(n => (double)random.Next(-10, 10)))
                                : new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)random.Next(-10, 10)));

        (Matrix answer, char sign) = mode switch
        {
            0 => (mat1 + mat2, '+'),
            1 => (mat1 - mat2, '-'),
            _ => (mat1 * mat2, '*'),
        };

        return new MatricesProblem(mat1, mat2, sign, answer);
    }

    public MatricesProblemGenerator() : this(new Random()) { }
    public MatricesProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}