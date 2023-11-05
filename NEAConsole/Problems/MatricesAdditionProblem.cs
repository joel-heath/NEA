using NEAConsole.Matrices;

namespace NEAConsole.Problems;

public class MatricesAdditionProblem : IProblem
{
    private readonly Matrix mat1;
    private readonly Matrix mat2;
    private readonly Matrix solution;
    private readonly char operand;

    public void Display()
    {
        UIMethods.DrawMatrix(mat1);

        var signSpacing = (mat1.Rows - 1) / 2;
        Console.CursorTop += signSpacing;
        Console.Write($" {operand} ");
        Console.CursorTop -= signSpacing;
        UIMethods.DrawMatrix(mat2);

        Console.CursorTop += signSpacing;
        Console.Write($" = ");
        Console.CursorTop -= signSpacing;
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = InputMethods.InputMatrix(solution.Rows, solution.Columns, (oldAnswer as MatrixAnswer)?.Answer, ct);
        Console.WriteLine();
        return new MatrixAnswer(answer);
    }

    public void DisplayAnswer(IAnswer answer)
    {
        UIMethods.DrawMatrix((answer as MatrixAnswer ?? throw new InvalidOperationException()).Answer, false);
        Console.WriteLine();
    }
    public bool EvaluateAnswer(IAnswer answer)
        => (answer as MatrixAnswer ?? throw new InvalidOperationException()).Answer == solution;

    public void Summarise(IAnswer? answer)
    {
        bool correct;
        try { correct = answer is not null && EvaluateAnswer(answer); }
        catch (InvalidOperationException) { correct = false; }
        if (correct)
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            if (answer is null)
            {
                Console.CursorTop += mat1.Columns;
                Console.WriteLine();
            }
            Console.WriteLine("Incorrect. The correct answer was: ");
            UIMethods.DrawMatrix(solution, false);
            Console.WriteLine();
        }
    }

    public MatricesAdditionProblem(Matrix mat1, Matrix mat2, char operand, Matrix solution)
    {
        this.mat1 = mat1;
        this.mat2 = mat2;
        this.operand = operand;
        this.solution = solution;
    }
}