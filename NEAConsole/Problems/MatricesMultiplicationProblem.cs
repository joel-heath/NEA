using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesMultiplicationProblem : IProblem
{
    private readonly Matrix mat1;
    private readonly Matrix mat2;
    private readonly Matrix solution;

    public void Display()
    {
        UIMethods.DrawMatrix(mat1);

        var signSpacing = (mat1.Rows - 1) / 2;
        Console.CursorTop += signSpacing;
        Console.Write($" x ");
        Console.CursorTop -= signSpacing;
        UIMethods.DrawMatrix(mat2);

        Console.CursorTop += signSpacing;
        Console.Write($" = ");
        Console.CursorTop -= signSpacing;
    }

    public IAnswer GetAnswer()
    {
        var answer = UIMethods.InputMatrix(solution.Rows, solution.Columns);
        Console.WriteLine();

        return new MatrixAnswer(answer);
    }

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as MatrixAnswer ?? throw new InvalidOperationException()).Answer == solution; 

    public void Summarise(IAnswer answer)
    {
        if (EvaluateAnswer(answer))
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine("Incorrect. The correct answer was: ");
            UIMethods.DrawMatrix(solution, false);
            Console.WriteLine();
        }

        UIMethods.Wait();
        Console.Clear();
    }

    public MatricesMultiplicationProblem(Matrix mat1, Matrix mat2, Matrix solution)
    {
        this.mat1 = mat1;
        this.mat2 = mat2;
        this.solution = solution;
    }
}