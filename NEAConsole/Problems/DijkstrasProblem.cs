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
        Console.WriteLine();
        DrawMatrix(graph);
        //DebugDrawMatrix(graph);
    }

    public void GetAnswer()
    {
        //Console.WriteLine("Path taken (abc...) ");
        Console.WriteLine();
        Console.Write("Total weight: ");
        answer = int.Parse(Console.ReadLine());
    }

    public bool EvaluateAnswer()
        => answer == solution;

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

    private static int[] GetMatrixWidths(Matrix m)
    {
        int[] widths = new int[m.Columns];
        for (int c = 0; c < m.Columns; c++)
        {
            int max = 0;
            for (int r = 0; r < m.Rows; r++)
            {
                var len = m[r, c].ToString().Length;
                if (len > max) max = len;
            }

            widths[c] = max;
        }

        return widths;
    }

    private static void DrawMatrix(Matrix m)
    {
        var widths = GetMatrixWidths(m);

        int xIndent = Console.CursorLeft;
        int initY = Console.CursorTop;
        // Column titles
        Console.Write("   ");
        for (int i = 0; i < m.Columns; i++)
        {
            var name = (char)('A' + i);
            var spaces = (widths[i] - 1) / 2;
            Console.Write($"{new string(' ', spaces)}{name}{new string(' ', widths[i] - spaces)}");
        }
        Console.WriteLine();
        for (int i = 0; i < m.Rows; i++)
        {
            //                  Row title
            Console.Write($"{(char)('A' + i)} [");
            for (int j = 0; j < m.Columns; j++)
            {
                string val = m[i, j] == 0 ? "-" : m[i,j].ToString();
                var len = val.Length;
                var spaces = (widths[j] - len) / 2;
                Console.Write($"{new string(' ', spaces)}{val}{new string(' ', widths[j] - spaces - len)}{(j < m.Columns - 1 ? " " : "]")}");
            }
            if (i < m.Rows - 1)
            {
                Console.CursorLeft = xIndent;
                Console.CursorTop++;
            }
        }
        Console.WriteLine();
    }

    static void DebugDrawMatrix(Matrix m)
    {
        for (int i = 0; i < m.Rows; i++)
        {
            for (int j = 0; j < m.Columns; j++)
            {
                Console.Write($"{m[i, j]},");
            }
            Console.CursorLeft--;
            Console.WriteLine(' ');
        }
    }

    public DijkstrasProblem(Matrix graph, char startNode, char endNode, int solution)
    {
        this.graph = graph;
        this.startNode = startNode;
        this.endNode = endNode;
        this.solution = solution;
    }
}