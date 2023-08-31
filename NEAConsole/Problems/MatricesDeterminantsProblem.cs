using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesDeterminantsProblem : IProblem
{
    private readonly Matrix matrix;
    private readonly double solution;
    private double? answer;

    public void Display()
    {
        var signSpacing = (matrix.Rows - 1) / 2;

        Console.CursorTop += signSpacing;
        Console.Write("det");
        Console.CursorTop -= signSpacing;
        MatricesProblem.DrawMatrix(matrix);

        Console.CursorTop += signSpacing;
        Console.Write(" = ");
    }

    public void GetAnswer()
    {
        answer = UIMethods.ReadInt();
        Console.WriteLine();
    }

    public bool EvaluateAnswer()
        => (answer ?? throw new NotAnsweredException()) == solution;

    public void Summarise()
    {
        if (EvaluateAnswer())
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine($"Incorrect. The correct answer was {solution}.");
        }

        Console.ReadKey(true);
        Console.Clear();
    }

    public MatricesDeterminantsProblem(Matrix matrix, double solution)
    {
        this.matrix = matrix;
        this.solution = solution;
    }
}