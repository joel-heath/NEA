using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesInversionProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrix Inversion";
    public string SkillPath => "Matrices.Determinants.Inversion";
    private readonly Random random;

    public IProblem Generate()
    {
        int rows = random.Next(2, 4);

        // somehow ensure matrix has determinant != 0
        // specifically make one with determinant = +-1 so that inverse is whole numbers

        Matrix mat = new(rows, rows, Enumerable.Range(0, rows * rows).Select(n => (double)random.Next(-7, 8)));

        Matrix answer = mat.Inverse;

        return new MatricesInversionProblem(mat, answer);
    }

    public MatricesInversionProblemGenerator() : this(new Random()) { }
    public MatricesInversionProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}