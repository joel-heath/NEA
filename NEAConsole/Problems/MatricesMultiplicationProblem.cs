using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class MatricesMultiplicationProblem : IProblem
{
    private readonly Matrix mat1;
    private readonly Matrix mat2;
    private readonly Matrix solution;
    private Matrix? answer;

    public void Display()
    {
        MatricesProblem.DrawMatrix(mat1);

        var signSpacing = (mat1.Rows - 1) / 2;
        Console.CursorTop += signSpacing;
        Console.Write($" x ");
        Console.CursorTop -= signSpacing;
        MatricesProblem.DrawMatrix(mat2);

        Console.CursorTop += signSpacing;
        Console.Write($" = ");
        Console.CursorTop -= signSpacing;
    }

    public void GetAnswer()
    {
        answer = MatricesProblem.InputMatrix(solution.Rows, solution.Columns);
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
            Console.WriteLine("Incorrect. The correct answer was: ");
            MatricesProblem.DrawMatrix(solution);
        }

        Console.ReadKey(true);
        Console.Clear();
    }

    public MatricesMultiplicationProblem(Matrix mat1, Matrix mat2, Matrix solution)
    {
        this.mat1 = mat1;
        this.mat2 = mat2;
        this.solution = solution;
    }
}