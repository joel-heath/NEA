using NEAConsole.Matrices;

namespace NEAConsole;
public static class UIMethods
{
    public static int ReadInt()
    {
        bool entering = true;
        string rawNum = string.Empty;
        int pos = 0;
        int indent = Console.CursorLeft;
        while (entering)
        {
            Console.CursorLeft = indent + pos;
            var k = Console.ReadKey(true);
            if (k.KeyChar >= '0' && k.KeyChar <= '9')
            {
                Console.Write(k.KeyChar);
                Console.Write(rawNum[pos..]);
                rawNum = rawNum[..pos] + k.KeyChar + rawNum[pos..];
                pos++;
                continue;
            }
            switch (k.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (pos > 0) pos--;
                    break;
                case ConsoleKey.RightArrow:
                    if (pos < rawNum.Length) pos++;
                    break;
                case ConsoleKey.Home:
                    pos = 0;
                    break;
                case ConsoleKey.End:
                    pos = rawNum.Length;
                    break;
                case ConsoleKey.Delete:
                    if (pos < rawNum.Length)
                    {
                        Console.Write(rawNum[(pos + 1)..] + ' ');
                        rawNum = rawNum[..pos] + rawNum[(pos + 1)..];
                    }
                    break;
                case ConsoleKey.Backspace:
                    if (pos > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(rawNum[pos--..] + ' ');
                        rawNum = rawNum[..pos] + rawNum[(pos + 1)..];
                    }
                    break;
                //case ConsoleKey.Escape:
                //throw new EscapeException();
                case ConsoleKey.Enter:
                    entering = false;
                    break;
            }
        }

        return int.Parse(rawNum);
    }

    /// <summary>
    /// Draws a matrix to the console
    /// </summary>
    /// <param name="m">Matrix to be drawn</param>
    /// <param name="resetY">Defaults to true, if true Console.CursorTop will be reset to the top of the matrix, otherwise it'll be at the bottom of the matrix.</param>
    public static void DrawMatrix(Matrix m, bool resetY = true)
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

        if (resetY) Console.CursorTop = initY;
    }

    /// <summary>
    /// Draw matrix with row and column titles A, B, C, etc.
    /// </summary>
    /// <param name="m">Matrix to be drawn</param>
    public static void DrawTitledMatrix(Matrix m)
    {
        var widths = GetMatrixWidths(m);

        int xIndent = Console.CursorLeft;
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
                string val = m[i, j] == 0 ? "-" : m[i, j].ToString();
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

    /// <summary>
    /// Writes each element in the matrix in CSV form, commas separating columns and new lines separating rows. Exclusively for debugging.
    /// </summary>
    /// <param name="m">Matrix to be drawn.</param>
    public static void DebugDrawMatrix(Matrix m)
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

    /// <summary>
    /// Allows the user to input a matrix by drawing the matrix and allowing the user to navigate between elements and enter doubles.
    /// </summary>
    /// <param name="rows">Number of rows that the matrix the user may enter in will have.</param>
    /// <param name="cols">Number of columns that the matrix the user may enter in will have.</param>
    /// <returns>A matrix with the user's inputs.</returns>
    public static Matrix InputMatrix(int rows, int cols)
    {
        Console.CursorVisible = false;
        var initY = Console.CursorTop;

        Matrix? m = null!; // compilation purposes, wont ever return null

        string[][] inputs = InitialiseUserInputMatrix(rows, cols); // why is it a jagged array you ask? so i can select through it

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


                case ConsoleKey.Tab:
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

    private static void DrawInputMatrix(string[][] m, int x, int y)
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

    private static string[][] InitialiseUserInputMatrix(int rows, int cols)
    {
        // YES, Enumerable.Repeat(Enumerable.Repeat("", cols).ToArray(), rows).ToArray(); would work but that's inefficient

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

    public static int[] GetMatrixWidths(Matrix m)
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

    private static int[] GetInputMatWidths(string[][] m)
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
}