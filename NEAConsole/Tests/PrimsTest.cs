using NEAConsole.Matrices;
using System;

namespace NEAConsole.Tests;
internal class PrimsTest : ITest
{
    public string DisplayText => "Prim's Algorithm";
    private readonly Random random;

    public void Test()
    {
        var dimension = random.Next(8, 11);
        Matrix tree = new(dimension);

        for (int i = 1; i < dimension; i++)
        {
            // pick one of the already connected vertices to connect this new one to
            var connector = random.Next(0, i);

            var weight = random.Next(1, 16);

            tree[connector, i] = weight;
            tree[i, connector] = weight;
        }

        // 5x^2 - 85x + 367   (see 2. Robert J. Prim's algorithm -- pg 10)
        var edgesToAdd = 5 * dimension * dimension - 85 * dimension + 367;

        for (int i = 0; i < edgesToAdd; i++)
        {
            int node1 = 0, node2 = 0;
            while (tree[node1, node2] != 0)
            {
                node1 = random.Next(0, dimension);
                node2 = random.Next(0, dimension);
            }
            var weight = random.Next(1, 16);

            if (tree[node1, node2] != 0 || tree[node2, node1] != 0) throw new Exception("Did not successfully choose nodes that weren't already connected");

            tree[node1, node2] = weight;
            tree[node2, node1] = weight;
        }

        var solution = MatrixUtils.Prims(tree.ToMatrix(e => e == 0 ? double.MaxValue : e)).ToHashSet();

        // now need to give it to the user 

        Console.WriteLine("Apply Prim's algorithm to the following adjacency matrix to calculate the minimum spanning tree.");
        Console.WriteLine();

        var uinput = InputEdges(tree);

        Console.WriteLine();
        if (EvaluateAnswer(solution, uinput))
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine("Incorrect. The correct answer was: ");
            DrawMatrix(tree, -1, -1, solution);
        }

        Console.ReadKey(true);
        Console.Clear();
    }

    private static bool EvaluateAnswer(IReadOnlyCollection<(int row, int col)> solution, IReadOnlyCollection<(int row, int col)> uinput)
    {
        if (uinput.Count != solution.Count) return false;
        foreach (var e in solution)
        {
            if (!uinput.Contains(e))
            {
                return false;
            }
        }
        return true;
    }

    private static void DrawMatrix(Matrix m, int x, int y, IReadOnlyCollection<(int row, int col)> chosenEdges)
    {
        int xIndent = Console.CursorLeft;
        int yIndent = Console.CursorTop;

        var widths = GetMatrixWidths(m);

        for (int i = 0; i < m.Rows; i++)
        {
            Console.Write('[');
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

        Console.CursorTop = yIndent;
    }

    private static HashSet<(int row, int col)> InputEdges(Matrix adjacency)
    {
        Console.CursorVisible = false;
        var initY = Console.CursorTop;
        int x = 0, y = 0;

        HashSet<(int row, int col)> chosenEdges = new(adjacency.Rows * adjacency.Columns);
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


                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    if (adjacency[y, x] != 0 && !chosenEdges.Contains((y, x)))
                    {
                        chosenEdges.Add((y, x));
                    }
                    break;

                case ConsoleKey.Escape:
                    selecting = false;
                    break;

                case ConsoleKey.Backspace:
                    if (chosenEdges.Contains((y, x)))
                    {
                        chosenEdges.Remove((y,x));
                    }
                    break;
            }

            DrawMatrix(adjacency, x, y, chosenEdges);
        }

        Console.CursorVisible = true;
        Console.CursorTop = initY + adjacency.Rows;
        return chosenEdges;
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

    public PrimsTest() : this(new Random()) { }
    public PrimsTest(Random randomNumberGenerator) => random = randomNumberGenerator;
}