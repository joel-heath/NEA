namespace NEAConsole.Problems;

public class SortingProblem(int[] unsorted, int[] sorted, bool ascending, SortType sortingAlgorithm) : IProblem
{
    private readonly int[] unsorted = unsorted, sorted = sorted;
    private readonly SortType sortingAlgorithm = sortingAlgorithm;
    private readonly bool ascending = ascending;

    public void Display()
    {
        Console.WriteLine($"Apply the {sortingAlgorithm} to sort the following collection into {(ascending ? "a" : "de")}scending order." + Environment.NewLine);

        Console.WriteLine("Unsorted: " + string.Join(' ', unsorted));
        Console.Write("Sorted: ");
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null, CancellationToken? ct = null)
    {
        var answer = InputMethods.ReadList(startingVal: oldAnswer is ManyAnswer<int> oldAns ? string.Join(' ', oldAns.Answer) : string.Empty, ct: ct, options: new ReadValuesOptions { NewLine = true });

        return new ManyAnswer<int>([.. answer]);
    }

    public void DisplayAnswer(IAnswer answer)
        => Console.WriteLine(string.Join(' ', (answer as ManyAnswer<int> ?? throw new InvalidOperationException()).Answer));

    public bool EvaluateAnswer(IAnswer answer)
    {
        return (answer as ManyAnswer<int> ?? throw new InvalidOperationException()).Answer.Select((x, i) => x == sorted[i]).All(b => b);
    }

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
            Console.WriteLine($"Incorrect. The correct answer was: {string.Join(' ', sorted)}");
        }
    }
}