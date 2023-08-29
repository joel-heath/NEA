using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesDeterminantsProblem : IProblem
{
    private readonly Matrix matrix;
    private readonly double solution;
    private double? answer;

    public void Display()
    {
        Console.WriteLine("Evaluate the determinant of the following matrix. (to 3.s.f)");
        MatricesProblem.DrawMatrix(matrix);
    }

    public void GetAnswer()
    {
        answer = double.Parse(Console.ReadLine() ?? "0");
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