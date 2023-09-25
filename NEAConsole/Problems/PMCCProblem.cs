namespace NEAConsole.Problems;
internal class PMCCProblem : IProblem
{
    private readonly IList<(double x, double y)> data;
    private readonly double solution;

    public void Display()
    {
        Console.WriteLine("Find the PMCC of the following data to 3 s.f.\n");

        foreach (var item in data)
            Console.WriteLine(item);

        Console.Write("\nr = ");
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = UIMethods.ReadInt(startingNum: (oldAnswer as IntAnswer)?.Answer, ct: ct);

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

    public PMCCProblem(IList<(double x, double y)> data, double solution)
    {
        this.data = data;
        this.solution = solution;
    }
}