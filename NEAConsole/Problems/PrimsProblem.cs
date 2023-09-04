using NEAConsole.Matrices;

namespace NEAConsole.Problems;
internal class PrimsProblem : IProblem
{
    private readonly Matrix adjacencyMatrix;
    private readonly HashSet<(int row, int col)> solution;

    public void Display()
    {
        Console.WriteLine("Apply Prim's algorithm starting at vertex A to the following adjacency matrix to calculate the minimum spanning tree.");
        Console.WriteLine();
    }

    public IAnswer GetAnswer(IAnswer? oldAnswer = null)
    {
        var answer = InputEdges(adjacencyMatrix, (oldAnswer as PrimsAnswer)?.Answer);
        Console.WriteLine();
        Console.WriteLine();

        return new PrimsAnswer(answer);
    }

    public bool EvaluateAnswer(IAnswer answer)
    {
        var attempt = (answer as PrimsAnswer ?? throw new InvalidOperationException()).Answer;

        if (solution.Count != attempt.Count) return false;

        foreach (var e in solution)
        {
            if (!attempt.Contains(e) && !attempt.Contains((e.col, e.row))) // remember adjacency matrices are symmetric about the leading diagonal, both row,col and col,row are valid
            {
                return false;
            }
        }
        return true;
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
            Console.WriteLine("Incorrect. The correct answer was: ");
            DrawMatrix(adjacencyMatrix, -1, -1, solution, false);
            Console.WriteLine();
        }

        UIMethods.Wait();
        Console.Clear();
    }

    private static void DrawMatrix(Matrix m, int x, int y, IReadOnlyCollection<(int row, int col)> chosenEdges, bool resetY=true)
    {
        int xIndent = Console.CursorLeft;
        int yIndent = Console.CursorTop;

        var widths = UIMethods.GetMatrixWidths(m);

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
                if (i == y && j == x)
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                if (chosenEdges.Contains((i, j)))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                string value = m[i,j].ToString();
                if (value == "0") value = "-";
                var len = value.ToString().Length;
                var spaces = (widths[j] - len) / 2;
                Console.Write($"{new string(' ', spaces)}{value}{new string(' ', widths[j] - spaces - len)}");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(' ');
            }
            Console.CursorLeft--;
            Console.Write(']');
            Console.CursorLeft = xIndent;
            Console.CursorTop++;
        }

        if (resetY) Console.CursorTop = yIndent;
    }

    private static HashSet<(int row, int col)> InputEdges(Matrix adjacency, HashSet<(int row, int col)>? oldAnswer = null)
    {
        Console.CursorVisible = false;
        var initY = Console.CursorTop;
        int x = 0, y = 0;

        HashSet<(int row, int col)> chosenEdges = oldAnswer?.ToHashSet() ?? new(adjacency.Rows * adjacency.Columns);
        DrawMatrix(adjacency, x, y, chosenEdges);

        bool selecting = true;
        while (selecting)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    if (x < adjacency.Columns - 1) x++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (x > 0) x--;
                    break;
                case ConsoleKey.DownArrow:
                    if (y < adjacency.Rows - 1) y++;
                    break;
                case ConsoleKey.UpArrow:
                    if (y > 0) y--;
                    break;

                case ConsoleKey.Backspace:
                    if (chosenEdges.Contains((y, x)))
                    {
                        chosenEdges.Remove((y, x));
                    }
                    break;

                case ConsoleKey.Spacebar:
                    var coords = (y, x);
                    if (chosenEdges.Contains(coords))
                    {
                        chosenEdges.Remove(coords);
                    }
                    else if (adjacency[y, x] != 0)
                    {
                        chosenEdges.Add(coords);
                    }
                    break;

                case ConsoleKey.Enter:
                    if (chosenEdges.Count > 0)
                        selecting = false;
                    break;

                case ConsoleKey.Escape:
                    throw new EscapeException();
            }

            DrawMatrix(adjacency, x, y, chosenEdges);
        }

        Console.CursorVisible = true;
        Console.CursorTop = initY + adjacency.Rows;
        return chosenEdges;
    }

    public PrimsProblem(Matrix adjacencyMatrix, HashSet<(int row, int col)> solution)
    {
        this.adjacencyMatrix = adjacencyMatrix;
        this.solution = solution;
    }
}