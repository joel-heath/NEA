using System;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using NEAConsole.Matrices;

namespace NEAConsole;
public class Program
{
    public record struct MenuOption(string DisplayText, Action OnSelected)
    {
        public int Length => DisplayText.Length;
        public override readonly string ToString() => DisplayText;
        public static implicit operator MenuOption((string DisplayText, Action OnSelected) tuple) => new(tuple.DisplayText, tuple.OnSelected);
    }
    static void ListChoices(MenuOption[] options)
    {
        var yPos = Console.CursorTop;
        Console.WriteLine($"> {options[0]} <");
        for (int i = 1; i < options.Length; i++)
        {
            Console.WriteLine($"  {options[i]}  ");
        }
        //Console.WriteLine($"  Exit  ");

        Console.CursorTop = yPos;
    }

    static int Choose(MenuOption[] options)
    {
        //options = options.Append("Exit").ToArray();
        Console.CursorVisible = false;
        ListChoices(options);

        int choice = 0;
        bool choosing = true;
        while (choosing)
        {
            var changed = false;
            ConsoleKey key = Console.ReadKey(true).Key;
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

        Console.ReadKey(true);
        Console.Clear();
    }

    public record class SimplexInequality
    {
        public enum InequalityType { LessThan, GreaterThan, Equal }
        public int[] Coefficients { get; }
        public int Constant { get; set; }
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

        var constraints = new SimplexInequality[dimensions];

        for (int i = 0; i < dimensions; i++)
        {
            constraints[i] = CreateConstraint(dimensions, solution);
        }

        // make sure objective is integers (make constraints divisible by dimensions)
        for (int i = 0; i < dimensions; i++)
        {
            var sum = 0;
            for (int j = 0; j < dimensions; j++)
            {
                sum += constraints[j].Coefficients[i];
            }
            var remainder = dimensions - (sum % dimensions); // need to make sure we have integer objective
                                                                // objective is average of constraints, therefore must be divisible by dimensions
            // suppose sum of coefficients is 11 in a 3D LP. 11 % 3 == 2, 3 - 2 = 1, 11 + 1 = 12 now its divisible by 3.
            constraints[dimensions - 1].Coefficients[i] += remainder;
            constraints[dimensions - 1].Constant += remainder * solution[i];
        }

        SimplexInequality objective = GenerateObjectiveFunction(dimensions, solution, constraints);

        Console.WriteLine($"Maximise P = {objective.ToObjectiveString()}");
        Console.WriteLine($"Subject to:");

        for (int i = 0; i < dimensions; i++)
        {
            Console.WriteLine($"    {constraints[i]}");
        }

        Console.Write("\nP = ");
        var P = int.Parse(Console.ReadLine() ?? "0"); // need to catch potential input errors here
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
                Console.WriteLine((char)('x' + i) + " = " + solution[i]);
            }
        }
        else
        {
            Console.WriteLine("Correct!");
        }

        Console.ReadKey(true);
        Console.Clear();
    }

    public static SimplexInequality CreateConstraint(int dimensions, int[] solution)
    {
        int[] coeffs = new int[dimensions];
        var constant = 0;
        for (int j = 0; j < dimensions; j++)
        {
            var coefficient = Random.Shared.Next(1, 6);
            coeffs[j] = coefficient;
            constant += coefficient * solution[j];
        }

        return new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan);
    }

    private static SimplexInequality GenerateObjectiveFunction(int dimensions, int[] solution, IList<SimplexInequality> constraints)
    {
        int[] coeffs = new int[dimensions];
        var constant = 0;
        for (int i = 0; i < dimensions; i++)
        {
            int coefficient = 0; // AVERAGING TO AN INT!! WE MUST MAKE SURE CONTRAINTS AVERAGE TO AN INT
                                 // TODO: Generate n-1 rand numbers. gen nth rand number, mod sum by n, add result to final num.
            for (int j = 0; j < dimensions; j++)
            {
                coefficient += constraints[j].Coefficients[i];
            }

            if (coefficient % dimensions != 0) throw new Exception("Constraints are not divisible by the number of dimensions");
            coefficient /= dimensions;
            coeffs[i] = coefficient;
            constant += coefficient * solution[i];
        }

        return new SimplexInequality(coeffs, constant, SimplexInequality.InequalityType.LessThan); ;
    }

    static void MathsMenu()
    {

    }
    static void FMathsMenu()
    {
        MenuOption[] options = { ("Matrices", MatricesTest), ("Simplex", SimplexTest), ("Hypothesis Testing", CSciMenu), ("Dijkstra's", CSciMenu), ("Prim's", CSciMenu), ("Return", CSciMenu) };
        GenericMenu(options, "Choose a subject to revise");
        Console.Clear();
    }

    static void CSciMenu()
    {

    }

    static void Main(string[] args)
    {
        MenuOption[] options = { ("Maths", MathsMenu), ("Further Maths", FMathsMenu), ("Computer Science", CSciMenu) };
        GenericMenu(options, "Choose a subject to revise");
        Console.Clear();
    }


    static void GenericMenu(MenuOption[] options, string prompt)
    {
        bool @continue = true;
        while (@continue)
        {
            Console.WriteLine(prompt);
            var choice = Choose(options);
            if (choice >= options.Length)
            { 
                @continue = false; 
            }
            else
            {
                Console.Clear();
                options[choice].OnSelected();
            }
        }
    }
}