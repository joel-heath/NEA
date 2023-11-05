using NEAConsole.Matrices;

namespace NEAConsole.Problems;

public class MatricesDeterminantsProblem : IProblem
{
    private readonly Matrix matrix;
    private readonly double solution;

    public void Display()
    {
        var signSpacing = (matrix.Rows - 1) / 2;

        Console.CursorTop += signSpacing;
        Console.Write("det");
        Console.CursorTop -= signSpacing;
        UIMethods.DrawMatrix(matrix);

        Console.CursorTop += signSpacing;
        Console.Write(" = ");
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = InputMethods.ReadInt(startingNum: (oldAnswer as IntAnswer)?.Answer, ct: ct);
        Console.CursorTop += matrix.Rows - ((matrix.Rows - 1) / 2);

        return new IntAnswer(answer);
    }

    public void DisplayAnswer(IAnswer answer)
        => Console.WriteLine((answer as IntAnswer ?? throw new InvalidOperationException()).Answer);

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as IntAnswer ?? throw new InvalidOperationException()).Answer == solution;

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
            Console.WriteLine($"Incorrect. The correct answer was {solution}.");
        }
    }

    public MatricesDeterminantsProblem(Matrix matrix, double solution)
    {
        this.matrix = matrix;
        this.solution = solution;
    }
}