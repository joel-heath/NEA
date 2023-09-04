using NEAConsole.Matrices;
using System;

namespace NEAConsole.Problems;
internal class MatricesInversionProblem : IProblem
{
    private readonly Matrix mat;
    private readonly Matrix solution;

    public void Display()
    {
        UIMethods.DrawMatrix(mat);

        var signSpacing = (mat.Rows - 1) / 2;

        Console.Write("-1");
        Console.CursorTop += signSpacing;
        Console.Write(" = ");
        Console.CursorTop -= signSpacing;
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null)
    {
        var answer = UIMethods.InputMatrix(solution.Rows, solution.Columns, (oldAnswer as MatrixAnswer)?.Answer);
        Console.WriteLine();

        return new MatrixAnswer(answer);
    }

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as MatrixAnswer ?? throw new InvalidOperationException()).Answer == solution;

    public void Summarise(IAnswer? answer)
    {
        bool correct;
        try { correct = EvaluateAnswer(answer); }
        catch (InvalidOperationException) { correct = false; }
        if (correct)
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

    public MatricesInversionProblem(Matrix mat, Matrix solution)
    {
        this.mat = mat;
        this.solution = solution;
    }
}