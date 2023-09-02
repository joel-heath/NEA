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

    public IAnswer GetAnswer()
    {
        var answer = new int[solution.Length + 1];

        Console.Write("\nP = ");
        answer[solution.Length] = UIMethods.ReadInt(); // need to catch potential input errors here
        for (int i = 0; i < solution.Length; i++)
        {
            Console.Write((char)('x' + i) + " = ");
            answer[i] = UIMethods.ReadInt();
        }
        Console.WriteLine();

        return new SimplexAnswer(answer);
    }

    public bool EvaluateAnswer(IAnswer answer)
    {
        var attempt = (answer as SimplexAnswer ?? throw new InvalidOperationException()).Answer;
        return attempt[solution.Length] == objective.Constant
               && !attempt.Take(solution.Length - 1).Where((n, i) => n != solution[i]).Any(); // Why not just attempt.All()? Because it doesn't have an (item, index) overload
    }

    public void Summarise(IAnswer answer)
    {
        if (EvaluateAnswer(answer))
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

        UIMethods.Wait();
        Console.Clear();
    }

    public SimplexProblem(SimplexInequality objective, SimplexInequality[] constraints, int[] solution)
    {
        this.objective = objective;
        this.constraints = constraints;
        this.solution = solution;
    }
}