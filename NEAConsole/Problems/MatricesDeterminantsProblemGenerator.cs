using NEAConsole.Matrices;

namespace NEAConsole.Problems;

public class MatricesDeterminantsProblemGenerator(IRandom randomNumberGenerator) : IProblemGenerator
{
    public string DisplayText => "Matrix Determinants";
    public string SkillPath => "Matrices.Determinants";
    private readonly IRandom random = randomNumberGenerator;

    public IProblem Generate(Skill knowledge)
    {
        int dimension = random.Next(2, 3);
        Matrix matrix = new(dimension, dimension, Enumerable.Range(0, dimension * dimension).Select(n => (double)random.Next(-10, 10)));

        return new MatricesDeterminantsProblem(matrix, matrix.Determinant);
    }

    public MatricesDeterminantsProblemGenerator() : this(new Random()) { }
}