namespace NEAConsole.Problems;
internal class SimplexProblem : IProblem
{
    private readonly SimplexInequality objective;
    private readonly SimplexInequality[] constraints;
    private readonly int[] solution;
    private int[]? answer;

    public void Display()
    {

        Console.WriteLine($"Maximise P = {objective.ToObjectiveString()}");
        Console.WriteLine($"Subject to:");

        for (int i = 0; i < solution.Length; i++)
        {
            Console.WriteLine($"    {constraints[i]}");
        }
    }

    public void GetAnswer()
    {
        answer = new int[solution.Length + 1];

        Console.Write("\nP = ");
        answer[solution.Length] = int.Parse(Console.ReadLine() ?? "0"); // need to catch potential input errors here
        for (int i = 0; i < solution.Length; i++)
        {
            Console.Write((char)('x' + i) + " = ");
            answer[i] = int.Parse(Console.ReadLine() ?? "0");
        }
    }

    public bool EvaluateAnswer()
        => (answer ?? throw new NotAnsweredException())[solution.Length] == objective.Constant && !answer.Take(solution.Length-1).Where((n, i) => n != solution[i]).Any();

    public void Summarise()
    {
        if (EvaluateAnswer())
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

        Console.ReadKey(true);
        Console.Clear();
    }

    public SimplexProblem(SimplexInequality objective, SimplexInequality[] constraints, int[] solution)
    {
        this.objective = objective;
        this.constraints = constraints;
        this.solution = solution;
    }
}