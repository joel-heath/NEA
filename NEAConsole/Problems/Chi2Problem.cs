using NEAConsole.Matrices;

namespace NEAConsole.Problems;

public class Chi2Problem : IProblem
{
    private readonly Matrix data;
    private readonly double solution;

    public void Display()
    {
        Console.WriteLine("Find the chi-squared test statistic for the following observed frequencies to 3 d.p.");
        Console.WriteLine("You do not need to merge classes with expected frequencies that are less than 5." + Environment.NewLine);

        UIMethods.DrawMatrix(data, resetY: false);

        Console.WriteLine(Environment.NewLine);

        Console.Write("Chi-squared = ");
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = InputMethods.ReadDouble(startingNum: (oldAnswer as DoubleAnswer)?.Answer, ct: ct);

        return new DoubleAnswer(answer);
    }

    public void DisplayAnswer(IAnswer answer)
        => Console.WriteLine((answer as DoubleAnswer ?? throw new InvalidOperationException()).Answer);

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as DoubleAnswer ?? throw new InvalidOperationException()).Answer == Math.Round(solution, 3, MidpointRounding.AwayFromZero);

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

    public Chi2Problem(double[,] data, double solution)
    {
        this.data = new(data);
        this.solution = solution;
    }
}