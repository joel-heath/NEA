using NEAConsole.Matrices;
using System.Runtime.InteropServices;

namespace NEAConsole.Problems;
internal class DijkstrasProblem : IProblem
{
    private readonly Matrix graph;
    private readonly char startNode;
    private readonly char endNode;
    private readonly int solution; // distance
    private int? answer;

    public void Display()
    {
        Console.WriteLine($"Perform Dijkstra's algorithm on the graph represented by the following adjacency matrix to find the shortest path from {startNode} to {endNode} and it's total weight.");
        Console.WriteLine();
        UIMethods.DrawTitledMatrix(graph);
        //MatricesProblem.DebugDrawMatrix(graph);
    }

    public void GetAnswer()
    {
        //Console.WriteLine("Path taken (abc...) ");
        Console.WriteLine();
        Console.Write("Total weight: ");
        answer = UIMethods.ReadInt();
        Console.WriteLine();
    }

    public bool EvaluateAnswer()
        => (answer ?? throw new NotAnsweredException()) == solution;

    public void Summarise()
    {
        if (EvaluateAnswer())
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine($"Incorrect, the correct answer was {solution}");
        }

        Console.ReadKey(true);
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