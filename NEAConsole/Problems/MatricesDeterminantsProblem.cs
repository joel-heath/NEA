using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesDeterminantsProblem : IProblem
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

    public IAnswer GetAnswer()
    {
        var answer = UIMethods.ReadInt();
        Console.CursorTop += matrix.Rows - ((matrix.Rows - 1) / 2);

        return new IntAnswer(answer);
    }

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as IntAnswer ?? throw new InvalidOperationException()).Answer == solution;

    public void Summarise(IAnswer answer)
    {
        if (EvaluateAnswer(answer))
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine($"Incorrect. The correct answer was {solution}.");
        }

        UIMethods.Wait();
        Console.Clear();
    }

    public MatricesDeterminantsProblem(Matrix matrix, double solution)
    {
        this.matrix = matrix;
        this.solution = solution;
    }
}