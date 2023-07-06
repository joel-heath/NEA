using System;
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

    static void DrawMatrixBasic(Matrix m) // naive; needs centering cols based on number widths
    {
        int xIndent = Console.CursorLeft;
        int initY = Console.CursorTop;
        for (int i = 0; i < m.Rows; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m.Columns; j++)
            {
                Console.Write($"{m[i, j]}{(j == m.Columns - 1 ? ']' : " ")}");
            }
            if (i < m.Rows - 1)
            {
                Console.CursorLeft = xIndent;
                Console.CursorTop++;
            }
        }

        Console.CursorTop = initY;
    }

    static int[] GetMatrixWidths(Matrix m)
    {
        int[] widths = new int[m.Columns];
        for (int c = 0;c < m.Columns;c++)
        {
            int max = 0;
            for (int r = 0;r < m.Rows;r++)
            {
                var len = m[r, c].ToString().Length;
                if (len > max) max = len;
            }

            widths[c] = max;
        }

        return widths;
    }

    static void DrawMatrix(Matrix m)
    {
        var widths = GetMatrixWidths(m); // center each column based on width

        int xIndent = Console.CursorLeft;
        int initY = Console.CursorTop;
        for (int i = 0; i < m.Rows; i++)
        {
            Console.Write('[');
            for (int j = 0; j < m.Columns; j++)
            {
                Console.Write($"{m[i, j]}{(j == m.Columns - 1 ? ']' : " ")}");
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

                Console.Write(m[i][j] == "" ? " " : m[i][j]);
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(' ');
            }
            Console.CursorLeft--;
            Console.Write("] "); // write an extra space in case they backspaced and need to overite old bracket
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
        return m;
    }

    static void MatricesTest()
    {
        var mode = Random.Shared.Next(0, 3);

        (int rows, int cols) = (Random.Shared.Next(1, 4), Random.Shared.Next(1, 4));
        Matrix mat1 = new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)));
        Matrix mat2 = mode == 2 ? new(cols, Random.Shared.Next(0, 4), Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)))
                                : new(rows, cols, Enumerable.Range(0, rows * cols).Select(n => (double)Random.Shared.Next(-10, 10)));

        (Matrix answer, char sign) = mode switch
        {
            0 => (mat1 + mat2, '+'),
            1 => (mat1 - mat2, '-'),
            _ => (mat1 * mat2, '*'),
        };


        DrawMatrix(mat1);

        var signSpacing = (rows-1) / 2;
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
    }

    static void SimplexTest()
    {

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
                case 6: @continue = false; break;
            }

            if (menu is not null)
            {
                Console.Clear();
                menu.Invoke();
                menu = null;
            }
        }

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
            }
        }
    }
}