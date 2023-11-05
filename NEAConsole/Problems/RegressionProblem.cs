namespace NEAConsole.Problems;

public class RegressionProblem : IProblem
{
    private readonly IList<(double x, double y)> data;
    private readonly double[] solution;
    private readonly string[] solutionNames = { "m", "c" };


    public void Display()
    {
        Console.WriteLine("Find the y on x least squares regression line of the following data in the form y = mx + c, giving m and c to 3 s.f.\n");

        foreach (var item in data)
            Console.WriteLine(item);

        Console.WriteLine();
        Console.WriteLine();
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = InputMethods.ReadDoubles(solutionNames, startingVals: (oldAnswer as ManyAnswer<double>)?.Answer, ct: ct);

        return new ManyAnswer<double>(answer);
    }

    public void DisplayAnswer(IAnswer answer)
    {
        var ans = (answer as ManyAnswer<double> ?? throw new InvalidOperationException()).Answer;
        if (ans.Length != solutionNames.Length) throw new InvalidOperationException();

        for (int i = 0; i < ans.Length; i++)
        {
            Console.WriteLine($"{solutionNames[i]} = {ans[i]}");
        }
    }

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as ManyAnswer<double> ?? throw new InvalidOperationException()).Answer.Select((d, i) =>(d, i)).All((t) => t.d == Math.Round(solution[t.i], 3, MidpointRounding.AwayFromZero));

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
            Console.WriteLine($"Incorrect. The correct answer was:");
            for (int i = 0; i < solution.Length; i++)
            {
                Console.WriteLine($"{solutionNames[i]} = {solution[i]}");
            }
        }
    }

    public RegressionProblem(IList<(double x, double y)> data, (double m, double c) solution)
    {
        this.data = data;
        this.solution = new double[] { solution.m, solution.c };
    }
}