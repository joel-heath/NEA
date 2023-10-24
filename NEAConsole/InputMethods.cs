using NEAConsole.Matrices;

namespace NEAConsole;
public struct ReadValuesOptions
{
    public bool NewLine { get; set; }
    /// <summary>
    /// If DoubleRules is enabled, the user will be able to enter a negative sign but only at the start of a number.
    /// </summary>
    public bool IntRules { get; set; }
    /// <summary>
    /// If DoubleRules is enabled, the user will be able to enter a decimal point, but only one.
    /// </summary>
    public bool DoubleRules { get; set; }
}
public static class InputMethods
{
    public static void Wait(string message = "Press any key to continue...", CancellationToken? ct = null)
    {
        if (message.Length > 0)
            Console.WriteLine(message);

        if (ReadKey(true, ct).Key == ConsoleKey.Escape)
            throw new EscapeException();
    }

    public static ConsoleKeyInfo ReadKey(bool intercept = false, CancellationToken? ct = null)
    {
        if (ct is null) return Console.ReadKey(intercept);
        
        while (!ct.Value.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
                return Console.ReadKey(intercept);
        }

        throw new KeyNotFoundException();
    }

    public static int ReadInt(bool newLine = true, bool natural = true, int? startingNum = null, CancellationToken? ct = null)
        => ReadInts(new string[] { string.Empty }, startingNum is null ? null : new int[] { startingNum.Value }, natural, newLine, ct)[0];
    public static double ReadDouble(bool newLine = true, double? startingNum = null, CancellationToken? ct = null)
        => ReadDoubles(new string[] { string.Empty }, startingNum is null ? null : new double[] { startingNum.Value }, newLine, ct)[0];

    public static double[] ReadDoubles(string[] prompts, double[]? startingVals = null, bool newLine = true, CancellationToken? ct = null)
    {
        char[] validChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '-' };
        var vals = startingVals?.Select(x => x.ToString()).ToArray();
        Func<string, double> parser = double.Parse;
        var opts = new ReadValuesOptions() { DoubleRules = true, NewLine = newLine };

        return ReadValues(prompts, validChars, parser, vals, ct, opts);
    }

    public static int[] ReadInts(string[] prompts, int[]? startingVals = null, bool natural = true, bool newLine = true, CancellationToken? ct = null)
    {
        char[] validChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        var vals = startingVals?.Select(x => x.ToString()).ToArray();
        Func<string, int> parser = int.Parse;
        var opts = new ReadValuesOptions() { IntRules = !natural, NewLine = newLine };

        return ReadValues(prompts, validChars, parser, vals, ct, opts);
    }

    private static T[] ReadValues<T>(string[] prompts, char[] validChars, Func<string, T> parser, string[]? startingVals = null, CancellationToken? ct = null, ReadValuesOptions options = default)
    {
        bool entering = true;
        string[] uInputs = startingVals is null ? Enumerable.Repeat(string.Empty, prompts.Length).ToArray() : startingVals.ToArray();
        T[]? outputs = null;

        int x = uInputs[0].Length, y = 0;
        int yIndent = Console.CursorTop;
        int xIndent = Console.CursorLeft;

        for (int i = 0; i < prompts.Length; i++)
        {
            if (prompts[i] == string.Empty) continue;
            Console.WriteLine(prompts[i] += " = ");
        }

        while (entering)
        {
            Console.CursorTop = yIndent + y;
            Console.CursorLeft = xIndent + prompts[y].Length + x;

            var k = ReadKey(true, ct);

            if (validChars.Contains(k.KeyChar) || options.DoubleRules && k.KeyChar == '.' && !prompts[y].Contains('.') || options.IntRules && k.KeyChar == '-' && x == 0 && !prompts[y].Contains('-')) //(prompts[y].Length == 0 || prompts[y][0] != '-')))
            {
                Console.Write(k.KeyChar);
                Console.Write(uInputs[y][x..]);
                uInputs[y] = uInputs[y][..x] + k.KeyChar + uInputs[y][x..];
                x++;
                continue;
            }
            switch (k.Key)
            {
                case ConsoleKey.UpArrow:
                    if (y > 0)
                    {
                        y--;
                        x = Math.Min(x, uInputs[y].Length);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (y < uInputs.Length - 1)
                    {
                        y++;
                        x = Math.Min(x, uInputs[y].Length);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (x > 0) x--;
                    break;
                case ConsoleKey.RightArrow:
                    if (x < uInputs[y].Length) x++;
                    break;
                case ConsoleKey.Home:
                    x = 0;
                    break;
                case ConsoleKey.End:
                    x = uInputs[y].Length;
                    break;

                case ConsoleKey.Delete:
                    if (x < uInputs[y].Length)
                    {
                        Console.Write(uInputs[y][(x + 1)..] + ' ');
                        uInputs[y] = uInputs[y][..x] + uInputs[y][(x + 1)..];
                    }
                    break;
                case ConsoleKey.Backspace:
                    if (x > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(uInputs[y][x--..] + ' ');
                        uInputs[y] = uInputs[y][..x] + uInputs[y][(x + 1)..];
                    }
                    break;

                case ConsoleKey.Escape:
                    throw new EscapeException();

                case ConsoleKey.Enter:
                    outputs = new T[uInputs.Length];
                    try
                    {
                        for (int i = 0; i < uInputs.Length; i++)
                        {
                            var str = uInputs[i];
                            if (str.Length == 0)
                            {
                                outputs = null;
                                break;
                            }

                            outputs[i] = parser(str);
                        }
                    }
                    catch (OverflowException) { outputs = null; } // number too massive
                    catch (FormatException) { outputs = null; } // they put two decimal points or an f etc

                    if (outputs is not null)
                        entering = false;

                    break;
            }
        }
        Console.CursorTop = yIndent + prompts.Length;

        if (options.NewLine) Console.WriteLine();

        return outputs!;
    }

    public static string ReadLine(bool newLine = true, string? startingInput = null, CancellationToken? ct = null)
    {
        bool entering = true;
        string input = startingInput is null ? string.Empty : startingInput;
        int pos = input.Length;
        int indent = Console.CursorLeft;
        Console.Write(input);
        while (entering)
        {
            Console.CursorLeft = indent + pos;
            var k = ReadKey(true, ct);
            
            switch (k.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (pos > 0) pos--;
                    break;
                case ConsoleKey.RightArrow:
                    if (pos < input.Length) pos++;
                    break;
                case ConsoleKey.Home:
                    pos = 0;
                    break;
                case ConsoleKey.End:
                    pos = input.Length;
                    break;

                case ConsoleKey.Delete:
                    if (pos < input.Length)
                    {
                        Console.Write(input[(pos + 1)..] + ' ');
                        input = input[..pos] + input[(pos + 1)..];
                    }
                    break;
                case ConsoleKey.Backspace:
                    if (pos > 0)
                    {
                        Console.CursorLeft--;
                        Console.Write(input[pos--..] + ' ');
                        input = input[..pos] + input[(pos + 1)..];
                    }
                    break;

                case ConsoleKey.Escape:
                    throw new EscapeException();

                case ConsoleKey.Enter:
                    entering = false;
                    break;

                default:
                    if (k.KeyChar >= ' ' && k.KeyChar <= '~') // see ascii chart
                    {
                        Console.Write(k.KeyChar);
                        Console.Write(input[pos..]);
                        input = input[..pos] + k.KeyChar + input[pos..];
                        pos++;
                    }
                    break;
            }
        }
        if (newLine) Console.WriteLine();

        return input;
    }

    /// <summary>
    /// Allows the user to input a matrix by drawing the matrix and allowing the user to navigate between elements and enter doubles.
    /// </summary>
    /// <param name="rows">Number of rows that the matrix the user may enter in will have.</param>
    /// <param name="cols">Number of columns that the matrix the user may enter in will have.</param>
    /// <returns>A matrix with the user's inputs.</returns>
    public static Matrix InputMatrix(int rows, int cols, Matrix? startingInput = null, CancellationToken? ct = null)
    {
        Console.CursorVisible = false;
        var initY = Console.CursorTop;

        Matrix m = null!; // compilation purposes, wont ever return null

        // why is it a jagged array you ask? so i can select through it
        string[][] inputs = startingInput is null ? InitialiseUserInputMatrix(rows, cols) : InitialiseUserInputMatrix(startingInput);

        int x = 0, y = 0;
        DrawInputMatrix(inputs, x, y);

        bool entering = true;
        while (entering)
        {
            var key = ReadKey(true, ct);
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

                case ConsoleKey.Backspace:
                    if (inputs[y][x].Length > 0)
                    {
                        inputs[y][x] = inputs[y][x][..^1];
                    }
                    break;

                case ConsoleKey.Tab:
                case ConsoleKey.Spacebar:
                    if (x < cols - 1) x++;
                    else if (y < rows - 1) (x, y) = (0, y + 1);
                    else entering = false;
                    break;

                case ConsoleKey.Enter:
                    entering = false;
                    break;

                case ConsoleKey.Escape:
                    throw new EscapeException();

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

    private static string[][] InitialiseUserInputMatrix(Matrix m)
    {
        // YES, Enumerable.Repeat(Enumerable.Repeat("", cols).ToArray(), rows).ToArray(); would work but that's inefficient

        string[][] inputs = new string[m.Rows][];

        for (int i = 0; i < m.Rows; i++)
        {
            inputs[i] = new string[m.Columns];
            for (int j = 0; j < m.Columns; j++)
            {
                inputs[i][j] = m[i,j].ToString();
            }
        }

        return inputs;
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

    public static void UpdateAllSkills(IEnumerable<Skill> skills, IEnumerable<string>? skillPath = null)
    {
        skillPath ??= new List<string>();
        foreach (Skill skill in skills)
        {
            var newPath = skillPath.Append(skill.Name);
            Console.WriteLine($"Do you know {string.Join(" > ", newPath)}?");

            bool response = Menu.Affirm();
            Console.Clear();

            if (response) skill.Known = true;
            else continue;

            if (skill.Children.Length > 0)
            {
                UpdateAllSkills(skill.Children, newPath);
            }
        }
    }

    public static void UpdateKnownSkills(IEnumerable<Skill> skills, IEnumerable<string>? skillPath = null)
    {
        skillPath ??= Array.Empty<string>();
        foreach (Skill skill in skills.Where(s => s.Known))
        {
            var newPath = skillPath.Append(skill.Name);
            Console.WriteLine($"Do you want to be tested on {string.Join(" > ", newPath)}?");

            bool response = Menu.Affirm();
            Console.Clear();

            if (!response) skill.Known = false;

            if (skill.Children.Length > 0)
            {
                UpdateKnownSkills(skill.Children, skillPath); // or new path if we want Matrices > Determinants > Inverses, but we don't
            }
        }
    }
}