using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesDeterminantsProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrix Determinants";
    public string SkillPath => "Matrices.Determinants";
    private readonly Random random;

    public IProblem Generate(Skill knowledge)
    {
        int dimension = random.Next(2, 3);
        Matrix matrix = new(dimension, dimension, Enumerable.Range(0, dimension * dimension).Select(n => (double)random.Next(-10, 10)));

        return new MatricesDeterminantsProblem(matrix, matrix.Determinant);
    }

    public MatricesDeterminantsProblemGenerator() : this(new Random()) { }
    public MatricesDeterminantsProblemGenerator(Random randomNumberGenerator) => random = randomNumberGenerator;
}