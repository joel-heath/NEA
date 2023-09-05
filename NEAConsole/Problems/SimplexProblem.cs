namespace NEAConsole.Problems;
internal class SimplexProblem : IProblem
{
    private readonly SimplexInequality objective;
    private readonly SimplexInequality[] constraints;
    private readonly int[] solution;

    public void Display()
    {

        Console.WriteLine($"Maximise P = {objective.ToObjectiveString()}");
        Console.WriteLine($"Subject to:");

        for (int i = 0; i < solution.Length; i++)
        {
            Console.WriteLine($"    {constraints[i]}");
        }
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null) // would be nice to be able to navigate up and down like in a matrix
    {
        var answer = (oldAnswer as SimplexAnswer)?.Answer ?? new int[solution.Length + 1];

        Console.Write("\nP = ");
        answer[solution.Length] = UIMethods.ReadInt(startingNum:answer[solution.Length]); // need to catch potential input errors here
        for (int i = 0; i < solution.Length; i++)
        {
            Console.Write((char)('x' + i) + " = ");
            answer[i] = UIMethods.ReadInt(startingNum: answer[i]);
        }
        Console.WriteLine();

        return new SimplexAnswer(answer);
    }

    public void DisplayAnswer(IAnswer answer)
    {
        var attempt = (answer as SimplexAnswer ?? throw new InvalidOperationException()).Answer;
        Console.WriteLine("P = " + attempt[^1]);
        for (int i = 0; i < attempt.Length - 1; i++)
        {
            Console.WriteLine((char)('x' + i) + " = " + attempt[i]);
        }
    }

    public bool EvaluateAnswer(IAnswer answer)
    {
        var attempt = (answer as SimplexAnswer ?? throw new InvalidOperationException()).Answer;
        return attempt[solution.Length] == objective.Constant
               && !attempt.Take(solution.Length - 1).Where((n, i) => n != solution[i]).Any(); // Why not just attempt.All()? Because it doesn't have an (item, index) overload
    }

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
            Console.WriteLine("Incorrect, the correct answer was:");
            Console.WriteLine("P = " + objective.Constant);
            for (int i = 0; i < solution.Length; i++)
            {
                Console.WriteLine((char)('x' + i) + " = " + solution[i]);
            }
        }
    }

    public SimplexProblem(SimplexInequality objective, SimplexInequality[] constraints, int[] solution)
    {
        this.objective = objective;
        this.constraints = constraints;
        this.solution = solution;
    }
}