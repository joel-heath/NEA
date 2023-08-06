using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class DijkstrasProblem : IProblem
{
    private readonly Matrix graph;
    private readonly char startNode;
    private readonly char endNode;
    private readonly int solution; // distance
    private int answer;

    public void Display()
    {
        Console.WriteLine($"Perform Dijkstra's algorithm on the graph represented by the following adjacency matrix to find the shortest path from {startNode} to {endNode} and it's total weight.");
        Console.WriteLine($"Subject to:");
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