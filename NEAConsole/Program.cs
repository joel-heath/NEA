using System;
using System.Numerics;
using System.Security.AccessControl;
using NEAConsole.Matrices;

namespace NEAConsole;
internal class Program
{
    static void ListChoices(string[] options)
    {
        var yPos = Console.CursorTop;
        Console.WriteLine($"> {options[0]} <");
        for (int i = 1; i < options.Length; i++)
        {
            Console.WriteLine($"  {options[i]}  ");
        }

        Console.CursorTop = yPos;
    }

    static int Choose(string[] options)
    {
        Console.CursorVisible = false;
        ListChoices(options);

        int choice = 0;
        bool choosing = true;
        while (choosing)
        {
            var changed = false;
            ConsoleKey key = Console.ReadKey().Key;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (choice > 0)
                    {
                        Console.CursorLeft = 0;
                        Console.Write(' ');
                        Console.CursorLeft = options[choice].Length + 3;
                        Console.Write(' ');
                        Console.CursorTop--;
                        choice--;
                        changed = true;
                    }
                    break;

                case ConsoleKey.DownArrow:
                    if (choice < options.Length - 1)
                    {
                        Console.CursorLeft = 0;
                        Console.Write(' ');
                        Console.CursorLeft = options[choice].Length + 3;
                        Console.Write(' ');
                        Console.CursorTop++;
                        choice++;
                        changed = true;
                    }
                    break;

                case ConsoleKey.Enter:
                    choosing = false;
                    break;
            }

            if (changed)
            {
                Console.CursorLeft = 0;
                Console.Write('>');
                Console.CursorLeft = options[choice].Length + 3;
                Console.Write('<');
            }
        }

        Console.SetCursorPosition(0, 0);
        Console.CursorVisible = true;
        return choice;
    }

    static int[] GetMatrixWidths(Matrix m)
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

    static int[] GetInputMatWidths(string[][] m)
    {
        int[] widths = new int[m[0].Length];
        for (int c = 0; c < m[0].Length; c++)
        {
            int max = 0;
            for (int r = 0; r < m.Length; r++)
            {
                var val = m[r][c];

                var len = val == "" ? 1 : m[r][c].ToString().Length;
                if (len > max) max = len;
            }

            widths[c] = max;
        }

        return widths;
    }

    static void DrawMatrix(Matrix m)
    {
        var widths = GetMatrixWidths(m);

        int xIndent = Console.CursorLeft;
        int initY = Console.CursorTop;
        for (int i = 0; i < m.Rows; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m.Columns; j++)
            {
                var num = m[i, j];
                var len = num.ToString().Length;
                var spaces = (widths[j] - len) / 2;
                Console.Write($"{new string(' ', spaces)}{num}{new string(' ', widths[j] - spaces - len)}{(j < m.Columns - 1 ? " " : "]")}");
            }
            if (i < m.Rows - 1)
            {
                Console.CursorLeft = xIndent;
                Console.CursorTop++;
            }
        }

        Console.CursorTop = initY;
    }

    static void DrawInputMatrix(string[][] m, int x, int y)
    {
        int xIndent = Console.CursorLeft;
        int yIndent = Console.CursorTop;

        var widths = GetInputMatWidths(m);

        for (int i = 0; i < m.Length; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m[0].Length; j++)
            {
                if (i == y && j == x)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                var value = m[i][j];
                if (value == "") value = " ";
                var len = value.ToString().Length;
                var spaces = (widths[j] - len) / 2;
                Console.Write($"{new string(' ', spaces)}{value}{new string(' ', widths[j] - spaces - len)}");

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(' ');
            }
            Console.CursorLeft--;
            Console.Write("] "); // write an extra space in case they backspaced and need to overwrite old bracket
            Console.CursorLeft = xIndent;
            Console.CursorTop++;
        }

        Console.CursorTop = yIndent;
    }

    static string[][] EmptyInputMat(int rows, int cols) // the pains of a 2d array
    {
        string[][] inputs = new string[rows][];

        for (int i = 0; i < rows; i++)
        {
            inputs[i] = new string[cols];
            for (int j = 0; j < cols; j++)
            {
                inputs[i][j] = "";
            }
        }

        return inputs;
    }

    static Matrix InputMatrix(int rows, int cols)
    {
        Console.CursorVisible = false;
        var initY = Console.CursorTop;

        Matrix? m = null!; // compilation purposes, wont ever return null
        string[][] inputs = EmptyInputMat(rows, cols); // why is it a jagged array you ask? so i can select through it

        int x = 0, y = 0;
        DrawInputMatrix(inputs, x, y);

        bool entering = true;
        while (entering)
        {
            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    if (x < cols - 1) x++;
                    break;
                case ConsoleKey.LeftArrow:
                    if (x > 0) x--;
                    break;
                case ConsoleKey.DownArrow:
                    if (y < rows - 1) y++;
                    break;
                case ConsoleKey.UpArrow:
                    if (y > 0) y--;
                    break;


                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    if (x < cols - 1) x++;
                    else if (y < rows - 1) (x, y) = (0, y + 1);
                    else entering = false;
                    break;

                case ConsoleKey.Escape:
                    entering = false;
                    break;

                case ConsoleKey.Backspace:
                    if (inputs[y][x].Length > 0)
                    {
                        inputs[y][x] = inputs[y][x][..^1];
                    }
                    break;

                default:
                    inputs[y][x] += key.KeyChar;
                    break;

            }

            DrawInputMatrix(inputs, x, y);

            if (!entering)
            {
                try
                {
                    var doubles = inputs.Select(r => r.Select(e => double.Parse(e)));
                    m = new Matrix(rows, cols, doubles);
                }
                catch (FormatException)
                {
                    entering = true;
                }
            }
        }

        Console.CursorVisible = true;
        Console.CursorTop = initY + rows;
        return m;
    }

    static void MatricesTest()
    {
        var mode = Random.Shared.Next(0, 3);
        (int rows, int cols) = (Random.Shared.Next(1, 4), Random.Shared.Next(1, 4));

        Matrix mat1 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)));
        Matrix mat2 = mode == 2 ? new(cols, rows = Random.Shared.Next(0, 4), Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)))
                                : new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)));

        (Matrix answer, char sign) = mode switch
        {
            0 => (mat1 + mat2, '+'),
            1 => (mat1 - mat2, '-'),
            _ => (mat1 * mat2, '*'),
        };


        DrawMatrix(mat1);

        var signSpacing = (rows - 1) / 2;
        Console.CursorTop += signSpacing;
        Console.Write($" {sign} ");
        Console.CursorTop -= signSpacing;
        DrawMatrix(mat2);

        Console.CursorTop += signSpacing;
        Console.Write($" = ");
        Console.CursorTop -= signSpacing;

        Matrix input = InputMatrix(answer.Rows, answer.Columns);

        Console.WriteLine();
        if (input == answer)
        {
            Console.WriteLine("Correct!");
        }
        else
        {
            Console.WriteLine("Incorrect. The correct answer was: ");
            DrawMatrix(answer);
        }

        Console.ReadKey();
        Console.Clear();
    }

    static int GenerateUniqueGradient(HashSet<int> gradients)
    {
        int m = Random.Shared.Next(-5, 0);
        if (!gradients.Contains(m))
        {
            gradients.Add(m);
            return m; // compiler moment
        }

        bool newFound = false;
        while (!newFound)
        {
            m = Random.Shared.Next(-5, 0);
            if (!gradients.Contains(m)) newFound = true;
        }
        gradients.Add(m);
        return m;
    }

    public record class SimplexInequality
    {
        public enum InequalityType { LessThan, GreaterThan, Equal }
        public int[] Coefficients { get; }
        public int Constant { get; }
        public InequalityType Inequality { get; }

        public SimplexInequality(int[] coeffs, int constant, InequalityType inequality)
        {
            Coefficients = coeffs;
            Constant = constant;
            Inequality = inequality;
        }

        /// <summary>
        /// MAXIMUM 3 VARIABLES (x, y, z)
        /// Does NOT simplify a + -b to a - b
        /// </summary>
        /// <returns></returns>
        public override string ToString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"))
                                             + $" {(int)Inequality switch { 0 => "<=", 1 => ">=", _ => "=" }} {Constant}";
        public string ToObjectiveString() => string.Join(" + ", Coefficients.Select((c, i) => $"{c}{(char)(i + 'x')}"));

    }

    static void SimplexTest()
    {
        int dimensions = Random.Shared.Next(2, 4);
        int[] solution = Enumerable.Range(0, dimensions).Select(n => Random.Shared.Next(2, 6)).ToArray();

        SimplexInequality[] constraints = new SimplexInequality[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            int[] coeffs = new int[dimensions];
            var constant = 0;
            for (int j = 0; j < dimensions; j++)
            {
                var coefficient = Random.Shared.Next(1, 6);
                coeffs[j] = coefficient;
                constant += coefficient * solution[j];
            }

            constraints[i] = new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan);
        }

        SimplexInequality objective; // just would prefer these to be garbage collected :)
        {
            int[] coeffs = new int[dimensions];
            var constant = 0;
            for (int j = 0; j < dimensions; j++)
            {
                var coefficient = Random.Shared.Next(1, 6);
                coeffs[j] = coefficient;
                constant += coefficient * solution[j];
            }

            objective = new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan);
        }

        Console.WriteLine($"Maximise P = {objective.ToObjectiveString()}");
        Console.WriteLine($"Subject to:");

        for (int i = 0; i < dimensions; i++)
        {
            Console.WriteLine($"    {constraints[i]}");
        }

        Console.Write("\nP = ");
        var P = int.Parse(Console.ReadLine() ?? "0");
        var input = new int[dimensions];
        for (int i = 0; i < dimensions; i++)
        {
            Console.Write((char)('x' + i) + " = ");
            input[i] = int.Parse(Console.ReadLine() ?? "0");
        }
        
        if (input.Where((n, i) => n != solution[i]).Any())
        {
            Console.WriteLine("Incorrect, the correct answer was:");
            Console.WriteLine("P = " + objective.Constant);
            for (int i = 0; i < dimensions; i++)
            {
                Console.Write((char)('x' + i) + " = " + solution[i]);
            }
        }
        else
        {
            Console.WriteLine("Correct!");
        }

        Console.ReadKey();
    }

    static void SimplexTest2D()
    {
        (int x, int y) solution = (Random.Shared.Next(2, 6), Random.Shared.Next(2, 6));

        HashSet<int> gradients = new(4);

        int m = GenerateUniqueGradient(gradients);
        (int m, int c) l1 = (m, m * -solution.x + solution.y); // lines that go through solution
        m = GenerateUniqueGradient(gradients);
        (int m, int c) l2 = (m, m * -solution.x + solution.y); // lines that go through solution
        m = GenerateUniqueGradient(gradients);
        (int m, int c) l3 = (m, m * -solution.x + solution.y + 2); // lines that goes over solution (redundant, just makes question more complicated)

        m = -GenerateUniqueGradient(gradients);
        (int x, int y) objective = (m, 1); // objective line (no c)


        Console.WriteLine($"Maximise P = {objective.x}x + y");
        Console.WriteLine($"Subject to:");
        Console.WriteLine($"    {-l1.m}x + y <= {l1.c}");
        Console.WriteLine($"    {-l2.m}x + y <= {l2.c}");
        Console.WriteLine($"    {-l3.m}x + y <= {l3.c}");

        Console.Write("\nP = ");
        double P = double.Parse(Console.ReadLine() ?? "0");
        Console.Write("x = ");
        double x = double.Parse(Console.ReadLine() ?? "0");
        Console.Write("y = ");
        double y = double.Parse(Console.ReadLine() ?? "0");

        var correctP = objective.x * solution.x + objective.y * solution.y;
        if (x == solution.x && y == solution.y && P == correctP)
        {
            Console.WriteLine("Correct!");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("Incorrect, the correct answer was:");
            Console.WriteLine("P = " + correctP);
            Console.WriteLine("x = " + solution.x);
            Console.WriteLine("y = " + solution.y);
        }
    }

    static void MathsMenu()
    {

    }
    static void FMathsMenu()
    {
        string[] options = { "Matrices", "Simplex", "Hypothesis Testing", "Dijkstra's", "Prim's", "Return" };

        Console.WriteLine("Choose a topic to be tested on");

        bool @continue = true;
        Action? menu = null; // will be resolved
        while (@continue)
        {
            switch (Choose(options))
            {
                case 0: menu = MatricesTest; break;
                case 1: menu = SimplexTest; break;
                case 5: @continue = false; break;
            }

            if (menu is not null)
            {
                Console.Clear();
                menu.Invoke();
                menu = null;
            }
        }

        Console.Clear();
    }
    static void CSciMenu()
    {

    }

    static void Main(string[] args)
    {
        string[] options = { "Maths", "Further Maths", "Computer Science", "Exit" };

        Console.WriteLine("Choose a subject to revise");

        bool @continue = true;
        Action? menu = null; // will be resolved
        while (@continue)
        {
            switch (Choose(options))
            {
                case 0: menu = MathsMenu; break;
                case 1: menu = FMathsMenu; break;
                case 2: menu = CSciMenu; break;
                case 3: @continue = false; break;
            }

            if (menu is not null)
            {
                Console.Clear();
                menu.Invoke();
                menu = null;
                Console.WriteLine("Choose a subject to revise");
            }
        }
    }
}