using NEAConsole.Matrices;

namespace NEAConsole.Problems;

public class MatricesAdditionProblemGenerator : IProblemGenerator
{
    public string DisplayText => "Matrix Addition";
    public string SkillPath => "Matrices.Addition";
    private readonly IRandom random;

    public IProblem Generate(Skill knowledge)
    {
        var mode = random.Next(0, 2);
        (int rows, int cols) = (random.Next(1, 4), random.Next(1, 4));

        Matrix mat1 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)random.Next(-10, 10)));
        Matrix mat2 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)random.Next(-10, 10)));

        (Matrix answer, char sign) = mode == 0 ? (mat1 + mat2, '+') : (mat1 - mat2, '-');

        return new MatricesAdditionProblem(mat1, mat2, sign, answer);
    }

    public MatricesAdditionProblemGenerator() : this(new Random()) { }
    public MatricesAdditionProblemGenerator(IRandom randomNumberGenerator) => random = randomNumberGenerator;
}