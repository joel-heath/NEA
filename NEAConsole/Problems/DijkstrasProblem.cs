using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class DijkstrasProblem : IProblem
{
    private readonly Matrix graph;
    private readonly char startNode;
    private readonly char endNode;
    private readonly int solution; // distance

    public void Display()
    {
        Console.WriteLine($"Perform Dijkstra's algorithm on the graph represented by the following adjacency matrix to find the shortest path from {startNode} to {endNode} and it's total weight.");
        Console.WriteLine();
        UIMethods.DrawTitledMatrix(graph);
#if DEBUG
        UIMethods.DebugDrawMatrix(graph);
#endif
    }

    public IAnswer GetAnswer()
    {
        //Console.WriteLine("Path taken (abc...) ");
        Console.WriteLine();
        Console.Write("Total weight: ");
        var answer = UIMethods.ReadInt();
        Console.WriteLine();

        return new IntAnswer(answer);
    }

    public bool EvaluateAnswer(IAnswer answer)
        => (answer as IntAnswer ?? throw new InvalidOperationException()).Answer == solution;
    public void Summarise(IAnswer answer)
    {
        if (EvaluateAnswer(answer))
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine($"Incorrect, the correct answer was {solution}");
        }

        UIMethods.Wait();
        Console.Clear();
    }

    public DijkstrasProblem(Matrix graph, char startNode, char endNode, int solution)
    {
        this.graph = graph;
        this.startNode = startNode;
        this.endNode = endNode;
        this.solution = solution;
    }
}